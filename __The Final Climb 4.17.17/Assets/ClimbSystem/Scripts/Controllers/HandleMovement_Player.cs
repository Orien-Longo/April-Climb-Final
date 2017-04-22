﻿using UnityEngine;
using System.Collections;

namespace TPC
{
    public class HandleMovement_Player : MonoBehaviour
    {
        StateManager states;
        Rigidbody rb;

        public bool doAngleCheck = true;
        [SerializeField]
        float degreesRunThreshold = 8;
        [SerializeField]
        bool useDot = true;

        bool overrideForce;
        bool inAngle;

        float rotateTimer_;
        float velocityChange = 4;
        bool applyJumpForce;

        Vector3 storeDirection;
        InputHandler ih;

        Vector3 curVelocity;
        Vector3 targetVelocity;
        float prevAngle;
        Vector3 prevDir;

        Vector3 overrideDirection;
        float overrideSpeed;
        float forceOverrideTimer;
        float forceOverLife;
        bool stopVelocity;
        bool useForceCurve;
        AnimationCurve forceCurve;
        float fc_t;
        bool initVault;
        Vector3 startPosition;


        bool forceOverHasRan;
        delegate void ForceOverrideStart();
        ForceOverrideStart forceOverStart;
        delegate void ForceOverrideWrap();
        ForceOverrideWrap forceOverWrap;

        BezierCurve climbCurve;
        bool enableRootMovement;
        
        public void Init(StateManager st, InputHandler inh)
        {
            ih = inh;
            states = st;
            rb = st.rBody;
            states.anim.applyRootMotion = false;

            GameObject curvePrefab = Resources.Load("CurveHolder_TPC") as GameObject;
            GameObject go = Instantiate(curvePrefab) as GameObject;
            climbCurve = go.GetComponentInChildren<BezierCurve>();
        }

        public void Tick()
        {
            if (states.curState == StateManager.CharStates.vaulting)
            {
                if (!initVault)
                {
                    VaultLogicInit();
                    initVault = true;
                }
                else
                {
                    HandleVaulting();
                }
                return;
            }

            if (!overrideForce && !initVault)
            {
                HandleDrag();
                if (states.onLocomotion)
                    MovementNormal();
                HandleJump();
            }
            else
            {
                states.horizontal = 0;
                states.vertical = 0;
                states.anim.SetFloat(Statics.horizontal, states.horizontal);
                states.anim.SetFloat(Statics.vertical, states.vertical);
                OverrideLogic();
            }

        }

        void MovementNormal()
        {
            inAngle = states.inAngle_MoveDir;

            Vector3 v = ih.camManager.transform.forward * states.vertical;
            Vector3 h = ih.camManager.transform.right * states.horizontal;

            v.y = 0;
            h.y = 0;

            if (states.onGround)
            {
                if (states.onLocomotion)
                    HandleRotation_Normal(h, v);

                float targetSpeed = states.walkSpeed;

                if (states.run && states.groundAngle == 0
                    && states.anim.GetBool(Statics.onSprint))
                {
                    targetSpeed = states.runSpeed;
                }

                if (inAngle)
                    HandleVelocity_Normal(h, v, targetSpeed);
                else
                    rb.velocity = Vector3.zero;
            }

            HandleAnimations_Normal();

        }

        void HandleVelocity_Normal(Vector3 h, Vector3 v, float speed)
        {
            Vector3 curVelocity = rb.velocity;

            if (states.curState == StateManager.CharStates.moving)
            {
                targetVelocity = (h + v).normalized * speed;
                velocityChange = 3;
            }
            else
            {
                velocityChange = 2;
                targetVelocity = Vector3.zero;
            }

            Vector3 vel = Vector3.Lerp(curVelocity, targetVelocity, Time.deltaTime * velocityChange);
            rb.velocity = vel;

            if (states.obstacleForward)
                rb.velocity = Vector3.zero;
        }

        void HandleRotation_Normal(Vector3 h, Vector3 v)
        {
            if (states.curState == StateManager.CharStates.moving)
            {
                storeDirection = (v + h).normalized;

                float targetAngle = Mathf.Atan2(storeDirection.x, storeDirection.z) * Mathf.Rad2Deg;

                if (states.run && doAngleCheck)
                {
                    if (!useDot)
                    {
                        if ((Mathf.Abs(prevAngle - targetAngle)) > degreesRunThreshold)
                        {
                            prevAngle = targetAngle;
                            PlayAnimSpecial(AnimSpecials.runToStop, false);
                            return;
                        }
                    }
                    else
                    {
                        float dot = Vector3.Dot(prevDir, states.moveDirection);
                        if (dot < 0)
                        {
                            prevDir = states.moveDirection;
                            PlayAnimSpecial(AnimSpecials.runToStop, false);
                            return;
                        }
                    }
                }

                prevDir = states.moveDirection;
                prevAngle = targetAngle;

                storeDirection += transform.position;
                Vector3 targetDir = (storeDirection - transform.position).normalized;
                targetDir.y = 0;
                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, velocityChange * Time.deltaTime);
            }
        }

        void HandleAnimations_Normal()
        {
            Vector3 relativeDirection = transform.InverseTransformDirection(states.moveDirection);

            float h = relativeDirection.x;
            float v = relativeDirection.z;

            if (states.obstacleForward)
                v = 0;

            states.anim.SetFloat(Statics.vertical, v, 0.2f, Time.deltaTime);
            states.anim.SetFloat(Statics.horizontal, h, 0.2f, Time.deltaTime);
        }

        void HandleJump()
        {
            if (states.onGround && states.canJump)
            {
                if (states.jumpInput && !states.jumping && states.onLocomotion
                    && states.curState != StateManager.CharStates.hold && states.curState != StateManager.CharStates.onAir)
                {
                    if (states.curState == StateManager.CharStates.idle || states.obstacleForward)
                    {
                        states.anim.SetBool(Statics.special, true);
                        states.anim.SetInteger(Statics.specialType, Statics.GetAnimSpecialType(AnimSpecials.jump_idle));
                    }

                    if (states.curState == StateManager.CharStates.moving && !states.obstacleForward)
                    {
                        states.LegFront();
                        states.jumping = true;
                        states.anim.SetBool(Statics.special, true);
                        states.anim.SetInteger(Statics.specialType, Statics.GetAnimSpecialType(AnimSpecials.run_jump));
                        states.curState = StateManager.CharStates.hold;
                        states.anim.SetBool(Statics.onAir, true);
                        states.canJump = false;
                    }
                }
            }

            if (states.jumping)
            {
                if (states.onGround)
                {
                    if (!applyJumpForce)
                    {
                        StartCoroutine(AddJumpForce(0));
                        applyJumpForce = true;
                    }
                }
                else
                {
                    states.jumping = false;
                }
            }
            else
            {

            }
        }

        IEnumerator AddJumpForce(float delay)
        {
            yield return new WaitForSeconds(delay);
            rb.drag = 0;
            Vector3 vel = rb.velocity;
            Vector3 forward = transform.forward;
            vel = forward * 3;
            vel.y = states.jumpForce;
            rb.velocity = vel;
            StartCoroutine(CloseJump());
        }

        IEnumerator CloseJump()
        {
            yield return new WaitForSeconds(0.3f);
            states.curState = StateManager.CharStates.onAir;
            states.jumping = false;
            applyJumpForce = false;
            states.canJump = false;
            StartCoroutine(EnableJump());
        }

        IEnumerator EnableJump()
        {
            yield return new WaitForSeconds(1.3f);
            states.canJump = true;
        }

        void HandleDrag()
        {
            if (states.curState == StateManager.CharStates.moving || states.onGround == false)
            {
                rb.drag = 0;
            }
            else
            {
                rb.drag = 4;
            }
        }

        public void PlayAnimSpecial(AnimSpecials t, bool sptrue = true)
        {
            int n = Statics.GetAnimSpecialType(t);
            states.anim.SetBool(Statics.special, sptrue);
            states.anim.SetInteger(Statics.specialType, n);
            StartCoroutine(CloseSpecialOnAnim(0.4f));
        }

        IEnumerator CloseSpecialOnAnim(float t)
        {
            yield return new WaitForSeconds(t);
            states.anim.SetBool(Statics.special, false);
        }

        bool canVault;//different from the states.canvault
        Vector3 targetVaultPosition;

        void VaultLogicInit()
        {
            canVault = states.canVault;
            states.canVault = false;
            VaultPhaseInit(states.targetVaultPosition);
        }

        void VaultPhaseInit(Vector3 targetPos)
        {
            states.controllerCollider.isTrigger = true;

            switch (states.curVaultType)
            {
                case StateManager.VaultType.idle:
                case StateManager.VaultType.walk:
                    overrideSpeed = Statics.vaultSpeedWalking; //How fast we basically vault
                    states.anim.CrossFade(Statics.walkVault, 0.1f);
                    break;
                case StateManager.VaultType.run:
                    overrideSpeed = Statics.vaultSpeedRunning; //How fast we basically vault
                    states.anim.CrossFade(Statics.runVault, 0.05f);
                    break;
                case StateManager.VaultType.walk_up:           
                    overrideSpeed = Statics.walkUpSpeed; 
                    if (!states.run)
                    { states.anim.CrossFade(Statics.walk_up, 0.05f); }
                    else
                    {
                        states.anim.CrossFade(Statics.run_up, 0.1f);
                        overrideSpeed = Statics.vaultSpeedRunning;
                    }
                    break;
                case StateManager.VaultType.climb_up:
                    states.anim.CrossFade(states.climbAnimName, 0.4f);
                    overrideSpeed = Statics.climbSpeed;
                    break;
            }

            int mirror = Random.Range(0, 2);
            states.anim.SetBool(Statics.mirrorJump, (mirror > 0));

            //let's reuse the same variables as for force override
            forceOverrideTimer = 0;
            //Since we're going to do a lerp, we don't need to use force over life for a timer
            //we know the lerp is over after the t has reached 1+
            //so we're gonna use the float as a container for the distance
            forceOverLife = Vector3.Distance(transform.position, targetPos);
            fc_t = 0;
          
            states.rBody.isKinematic = true;
            startPosition = transform.position;
            overrideDirection = targetPos - startPosition;
            overrideDirection.y = 0;
            targetVaultPosition = targetPos;

            if(states.curVaultType == StateManager.VaultType.climb_up)
            {
                startPosition = transform.position;
                targetVaultPosition = states.startVaultPosition;
                overrideDirection = targetPos - startPosition;
                overrideDirection.y = 0;
            }
        }

        public void HandleVaulting()
        {
            if (states.curVaultType == StateManager.VaultType.climb_up)
            {
                HandleCurveMovement();
                return;
            }

            //this will ensure the curve is sampled on the actual length of the lerp
            fc_t += Time.deltaTime;
            float targetSpeed = overrideSpeed * ih.vaultCurve.Evaluate(fc_t);

            forceOverrideTimer += Time.deltaTime * targetSpeed / forceOverLife;

            if(forceOverrideTimer > 1)
            {
                forceOverrideTimer = 1;
                StopVaulting();
            }

            Vector3 targetPosition = Vector3.Lerp(startPosition,
               targetVaultPosition, forceOverrideTimer);
            transform.position = targetPosition;

            //HandleRotation
            if (overrideDirection == Vector3.zero)
                overrideDirection = transform.forward;
            Quaternion targetRot = Quaternion.LookRotation(overrideDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5);
        }

        void StopVaulting()
        {
            states.curState = StateManager.CharStates.moving;
            states.vaulting = false;
            states.controllerCollider.isTrigger = false;
            states.rBody.isKinematic = false;
            states.skipGroundCheck = false;
            initVault = false;
            StartCoroutine("OpenCanVaultIfApplicable");
            isAtStart = false;
        }

        IEnumerator OpenCanVaultIfApplicable()
        {
            yield return new WaitForSeconds(0.4f);
            states.canVault = canVault;//enable it if the user has enabled it
        }

        public void AddVelocity(Vector3 direction,float t,float force, bool clamp, bool useFCurve, AnimationCurve fcurve)
        {
            forceOverLife = t;
            overrideSpeed = force;
            overrideForce = true;
            forceOverrideTimer = 0;
            overrideDirection = direction;
            rb.velocity = Vector3.zero;
            stopVelocity = clamp;
            forceCurve = fcurve;
            useForceCurve = useFCurve;
        }

        void OverrideLogic()
        {
            rb.drag = 0;

            if (!forceOverHasRan)//Run any delegates we have assigned on start
            {
                if (forceOverStart != null)
                    forceOverStart();

                forceOverHasRan = true;
            }

            float targetSpeed = overrideSpeed;

            if (useForceCurve)
            {
                fc_t += Time.deltaTime / forceOverLife;
                targetSpeed *= forceCurve.Evaluate(fc_t);
            }

            rb.velocity = overrideDirection * targetSpeed;

            forceOverrideTimer += Time.deltaTime;
            if(forceOverrideTimer > forceOverLife)
            {
                if(stopVelocity)
                    rb.velocity = Vector3.zero;

                stopVelocity = false;
                overrideForce = false;
                forceOverHasRan = false;

                if (forceOverWrap != null)//Run any delegates we have assigned on end
                    forceOverWrap();

                forceOverWrap = null;
                forceOverStart = null;
            }
        }

        public void EnableRootMovement()
        {
            enableRootMovement = true;
        }

        void InitClimbCurve()
        {
            startPosition = states.startVaultPosition;
            targetVaultPosition = states.targetVaultPosition;
            overrideDirection = targetVaultPosition - startPosition;
            overrideDirection.y = 0;
            enableRootMovement = false;
            climbCurve.transform.position = startPosition;
            climbCurve.transform.rotation = Quaternion.LookRotation(overrideDirection);
            BezierPoint[] points = climbCurve.GetAnchorPoints();
            points[0].transform.position = startPosition;
            points[points.Length - 1].transform.position = 
                targetVaultPosition + Vector3.up *0.1f;
            forceOverrideTimer = 0;
        }

        bool isAtStart;

        void HandleCurveMovement()
        {
            if (!isAtStart)
            {
                forceOverrideTimer += Time.deltaTime * 5;
                if (forceOverrideTimer > 1)
                {
                    forceOverrideTimer = 1;
                    InitClimbCurve();
                    isAtStart = true;
                }

                Vector3 targetPos = Vector3.Lerp(startPosition, targetVaultPosition, forceOverrideTimer);
                transform.position = targetPos;
            }
            else
            {
                if (enableRootMovement)
                {
                    forceOverrideTimer += Time.deltaTime * overrideSpeed;
                }

                if (forceOverrideTimer > 0.95f)
                {
                    forceOverrideTimer = 1;
                    StopVaulting();
                }

                Vector3 targetPos = climbCurve.GetPointAt(forceOverrideTimer);
                transform.position = targetPos;

                 if (overrideDirection == Vector3.zero)
                     overrideDirection = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(overrideDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5);
            }
        }
    }
}
