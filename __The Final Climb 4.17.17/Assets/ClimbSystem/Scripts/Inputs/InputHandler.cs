using UnityEngine;
using System.Collections;
using Controller;

namespace TPC
{
    public class InputHandler : MonoBehaviour
    {
        StateManager states;
        [HideInInspector]
        public CameraHandle camManager;
        HandleMovement_Player hMove;

        float horizontal;
        float vertical;

        [Header("Add a value to this in the inspector!")]
        public AnimationCurve vaultCurve;

        void Start()
        {
            //Add references
            gameObject.AddComponent<HandleMovement_Player>();

            //Get references
            camManager = CameraHandle.singleton;
            states = GetComponent<StateManager>();
            hMove = GetComponent<HandleMovement_Player>();

            camManager.target = this.transform;

            //Init in order
            states.isPlayer = true;
            states.Init();
            hMove.Init(states,this);

            FixPlayerMeshes();        
        }

        void FixPlayerMeshes()
        {
            SkinnedMeshRenderer[] skinned = GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skinned.Length; i++)
            {
                skinned[i].updateWhenOffscreen = true;
            }
        }

        void FixedUpdate()
        {
            states.FixedTick();
            UpdateStatesFromInput();
            if(!states.climbB.climbing)
                hMove.Tick();
        }

        void Update()
        {
            states.RegularTick();
        }

        void UpdateStatesFromInput()
        {
            vertical = Input.GetAxis(Statics.Vertical);
            horizontal = Input.GetAxis(Statics.Horizontal);

            Vector3 v = camManager.transform.forward * vertical;
            Vector3 h = camManager.transform.right * horizontal;

            v.y = 0;
            h.y = 0;

            states.horizontal = horizontal;
            states.vertical = vertical;

            Vector3 moveDir = (h + v).normalized;
            states.moveDirection = moveDir;
            states.inAngle_MoveDir = InAngle(states.moveDirection,25);
            if (states.walk && horizontal != 0 || states.walk && vertical != 0)
            {
                states.inAngle_MoveDir = InAngle(states.moveDirection, 60);
            }

            states.onLocomotion = states.anim.GetBool(Statics.onLocomotion);
            HandleRun();

            bool jInput = Input.GetButton(Statics.Jump);

            if(jInput)
            {
                if (!states.climbB.climbing)
                {
                    states.climbB.LookForClimbSpot();
                }
                else
                {
                    jInput = false;
                }          
            }

            states.jumpInput = jInput;
        }

        bool InAngle(Vector3 targetDir , float angleTheshold)
        {
            bool r = false;
            float angle = Vector3.Angle(transform.forward, targetDir);

            if (angle < angleTheshold)
            {
                r = true;
            }

            return r;
        }

        void HandleRun()
        {
            bool runInput = Input.GetButton(Statics.Fire3);

            if (runInput)
            {
                states.walk = false;
                states.run = true;
            }
            else
            {
                states.walk = true;
                states.run = false;
            }

            if (horizontal != 0 || vertical != 0)
            {
                states.run = runInput;
                states.anim.SetInteger(Statics.specialType, 
                    Statics.GetAnimSpecialType(AnimSpecials.run));
            }
            else
            {
                if(states.run)
                    states.run = false;
            }

            if (!states.inAngle_MoveDir && hMove.doAngleCheck)
                states.run = false;

            if (states.obstacleForward)
                states.run = false;

            if (states.run == false)
            {
                states.anim.SetInteger(Statics.specialType, Statics.GetAnimSpecialType(AnimSpecials.runToStop));
            }
        }

        public void EnableRootMovement()
        {
            hMove.EnableRootMovement();
        }
    }
}
