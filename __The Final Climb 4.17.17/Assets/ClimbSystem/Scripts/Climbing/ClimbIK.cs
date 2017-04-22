using UnityEngine;
using System.Collections;

namespace Climbing
{
    public class ClimbIK : MonoBehaviour
    {
        Animator anim;

        Point lhPoint;
        Point lfPoint;
        Point rhPoint;
        Point rfPoint;

        public float lh = 1;
        public float rh = 1;
        public float lf = 1;
        public float rf = 1;

        Transform lhHelper;
        Transform lfHelper;
        Transform rhHelper;
        Transform rfHelper;

        Vector3 lhTargetPosition;
        Vector3 lfTargetPosition;
        Vector3 rhTargetPosition;
        Vector3 rfTargetPosition;

        public float helperSpeed = 15;

        Transform hips;

        public bool forceFeetHeight;
        public PointStats pointD;//point defaults

        void Start()
        {
            anim = GetComponent<Animator>();
            hips = anim.GetBoneTransform(HumanBodyBones.Hips);

            lhHelper = new GameObject().transform;
            lhHelper.name = "lh helper ik";
            lfHelper = new GameObject().transform;
            lfHelper.name = "lf helper ik";
            rfHelper = new GameObject().transform;
            rfHelper.name = "rf helper ik";
            rhHelper = new GameObject().transform;
            rhHelper.name = "rh helper ik";
        }

        public void UpdateAllPointsOnOne(Point targetPoint)
        {
            curPointT = targetPoint.pointType;
            lhPoint = targetPoint;
            rhPoint = targetPoint;
            lfPoint = targetPoint;
            rfPoint = targetPoint;
        }

        public void UpdatePoint(AvatarIKGoal ik, Point targetPoint)
        {
            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    lfPoint = targetPoint;
                    break;
                case AvatarIKGoal.RightFoot:
                    rfPoint = targetPoint;
                    break;
                case AvatarIKGoal.LeftHand:
                    lhPoint = targetPoint;
                    break;
                case AvatarIKGoal.RightHand:
                    rhPoint = targetPoint;
                    break;
                default:
                    break;
            }
        }

        public void UpdateAllTargetPositions(Point p)
        {
            curPointT = p.pointType;

            lhTargetPosition = CreateIKPos(p, AvatarIKGoal.LeftHand,curPointT);
            rhTargetPosition = CreateIKPos(p, AvatarIKGoal.RightHand, curPointT);
            lfTargetPosition = CreateIKPos(p, AvatarIKGoal.LeftFoot, curPointT);
            rfTargetPosition = CreateIKPos(p, AvatarIKGoal.RightFoot, curPointT);
        }

        public void UpdateTargetPosition(AvatarIKGoal ik, Vector3 targetPosition)
        {
            switch (ik) 
            {
                case AvatarIKGoal.LeftFoot:
                    lfTargetPosition = targetPosition;
                    break;
                case AvatarIKGoal.RightFoot:
                    rfTargetPosition = targetPosition;
                    break;
                case AvatarIKGoal.LeftHand:
                    lhTargetPosition = targetPosition;
                    break;
                case AvatarIKGoal.RightHand:
                    rhTargetPosition = targetPosition;
                    break;
                default:
                    break;
            }
        }

        public Vector3 ReturnCurrentPointPosition(AvatarIKGoal ik)
        {
            Vector3 retVal = default(Vector3);

            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    retVal = CreateIKPos(lfPoint, AvatarIKGoal.LeftFoot, curPointT);
                    break;
                case AvatarIKGoal.RightFoot:
                    retVal = CreateIKPos(rfPoint, AvatarIKGoal.RightFoot, curPointT);
                    break;
                case AvatarIKGoal.LeftHand:
                    retVal = CreateIKPos(lhPoint, AvatarIKGoal.LeftHand, curPointT);
                    break;
                case AvatarIKGoal.RightHand:
                    retVal = CreateIKPos(rhPoint, AvatarIKGoal.RightHand, curPointT);
                    break;
                default:
                    break;
            }

            return retVal;
        }

        public Vector3 ReturnIKPosition_OnTargetPoint(Point p,AvatarIKGoal ik)
        {
            Vector3 retVal = default(Vector3);

            curPointT = p.pointType;

            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    retVal = CreateIKPos(p, AvatarIKGoal.LeftFoot, curPointT);
                    break;
                case AvatarIKGoal.RightFoot:
                    retVal = CreateIKPos(p, AvatarIKGoal.RightFoot, curPointT);
                    break;
                case AvatarIKGoal.LeftHand:
                    retVal = CreateIKPos(p, AvatarIKGoal.LeftHand, curPointT);
                    break;
                case AvatarIKGoal.RightHand:
                    retVal = CreateIKPos(p, AvatarIKGoal.RightHand, curPointT);
                    break;
                default:
                    break;
            }

            return retVal;
        }

        public Point ReturnPointForIK(AvatarIKGoal ik)
        {
            Point retVal = null;

            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    retVal = lfPoint;
                    break;
                case AvatarIKGoal.RightFoot:
                    retVal = rfPoint;
                    break;
                case AvatarIKGoal.LeftHand:
                    retVal = lhPoint;
                    break;
                case AvatarIKGoal.RightHand:
                    retVal = rhPoint;
                    break;
                default:
                    break;
            }

            if(retVal == null)
            {
                Debug.Log("Point for " + ik.ToString() + "is not assigned");
            }

            return retVal;
        }

        public AvatarIKGoal ReturnOppositeIK(AvatarIKGoal ik)
        {
            AvatarIKGoal retVal = default(AvatarIKGoal);

            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    retVal = AvatarIKGoal.RightFoot;
                    break;
                case AvatarIKGoal.RightFoot:
                    retVal = AvatarIKGoal.LeftFoot;
                    break;
                case AvatarIKGoal.LeftHand:
                    retVal = AvatarIKGoal.RightHand;
                    break;
                case AvatarIKGoal.RightHand:
                    retVal = AvatarIKGoal.LeftHand;
                    break;
                default:
                    break;
            }

            return retVal;
        }

        public AvatarIKGoal ReturnOppositeLimb(AvatarIKGoal ik)
        {
            AvatarIKGoal retVal = default(AvatarIKGoal);

            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    retVal = AvatarIKGoal.LeftHand;
                    break;
                case AvatarIKGoal.RightFoot:
                    retVal = AvatarIKGoal.RightHand;
                    break;
                case AvatarIKGoal.LeftHand:
                    retVal = AvatarIKGoal.LeftFoot;
                    break;
                case AvatarIKGoal.RightHand:
                    retVal = AvatarIKGoal.RightFoot;
                    break;
                default:
                    break;
            }

            return retVal;
        }

        public void AddWeightInfluenceAll(float w)
        {
            lh = w;
            lf = w;
            rh = w;
            rf = w;
        }

        public void ImmediatePlaceHelpers()
        {
            if(lhPoint != null)
            {
                lhHelper.position = lhTargetPosition;
            }

            if (rhPoint != null)
            {
                rhHelper.position = rhTargetPosition;
            }

            if(lfPoint != null)
            {
                lfHelper.position = lfTargetPosition;
            }

            if(rfPoint != null)
            {
                rfHelper.position = rfTargetPosition;
            }
        }

        void OnAnimatorIK()
        {  
            if (lhPoint)
            {
                //Left Hand
                IKPositions lhHolder = pointD.GetIKPos(AvatarIKGoal.LeftHand, curPointT);
                lhHelper.transform.position = Vector3.Lerp(lhHelper.transform.position, lhTargetPosition, Time.deltaTime * helperSpeed);               
                UpdateIK(AvatarIKGoal.LeftHand, lhHolder, lhHelper, lh, lhPoint);
            }

            if (rhPoint)
            {
                //Right Hand
                IKPositions rhHolder = pointD.GetIKPos(AvatarIKGoal.RightHand,curPointT);
                rhHelper.transform.position = Vector3.Lerp(rhHelper.transform.position, rhTargetPosition, Time.deltaTime * helperSpeed);      
                UpdateIK(AvatarIKGoal.RightHand, rhHolder, rhHelper, rh, rhPoint);
            }

            if (hips == null)
                hips = anim.GetBoneTransform(HumanBodyBones.Hips);

            if (lfPoint)
            {
                //Left Foot
                IKPositions lfHolder = pointD.GetIKPos(AvatarIKGoal.LeftFoot, curPointT);
                Vector3 targetPosition = lfTargetPosition;

                if (forceFeetHeight)
                {       if (targetPosition.y > hips.transform.position.y)
                    {
                        targetPosition.y = targetPosition.y - 0.2f;
                    }
                }

                lfHelper.transform.position = Vector3.Lerp(lfHelper.transform.position, targetPosition, Time.deltaTime * helperSpeed);              
                UpdateIK(AvatarIKGoal.LeftFoot, lfHolder, lfHelper, lf, lfPoint);
            }

            if (rfPoint)
            {
                //Right Foot
                IKPositions rfHolder = pointD.GetIKPos(AvatarIKGoal.RightFoot, curPointT);
                Vector3 targetPositon = rfTargetPosition;
                if (forceFeetHeight)
                {
                    if (targetPositon.y > hips.transform.position.y)
                    {
                        targetPositon.y = targetPositon.y - 0.2f;
                    }
                }

                rfHelper.transform.position = Vector3.Lerp(rfHelper.transform.position, targetPositon, Time.deltaTime * helperSpeed);               
                UpdateIK(AvatarIKGoal.RightFoot, rfHolder, rfHelper, rf, rfPoint);
            }
  
        }

        void UpdateIK(AvatarIKGoal ik, IKPositions holder,Transform helper, float weight, Point tP)
        {
            if (holder != null)
            {
                anim.SetIKPositionWeight(ik, weight);
                anim.SetIKRotationWeight(ik, weight);
                anim.SetIKPosition(ik, helper.position);
                anim.SetIKRotation(ik, helper.rotation);

                if (ik == AvatarIKGoal.LeftHand || ik == AvatarIKGoal.RightHand)
                {
                    Point p = ReturnPointForIK(ik);

                    switch (p.pointType)
                    {
                        case PointType.braced:
                        case PointType.hanging:
                        case PointType.ladder:
                            Transform shoulder = (ik == AvatarIKGoal.LeftHand) ?
                            anim.GetBoneTransform(HumanBodyBones.LeftShoulder) :
                            anim.GetBoneTransform(HumanBodyBones.RightShoulder);

                            //make the offset to be taken as a created position forward of the player
                            Vector3 offset = Vector3.zero;
                            offset += transform.forward;
                            offset += transform.up * 2.2f;
                            offset += transform.position;

                            Vector3 targetRotationDir = shoulder.transform.position - offset;

                            Quaternion targetRot = Quaternion.LookRotation(-targetRotationDir);
                            helper.rotation = targetRot;
                            break;
                        case PointType.column:
                            helper.parent = this.transform;
                            helper.localEulerAngles = pointD.GetIKPos(ik, p.pointType).ikRot;
                            helper.parent = null;
                            break;
                        default:
                            break;
                    }
                    
                }
                else
                {
                    helper.rotation = transform.rotation;
                }

                if (holder.hasHint)
                {
                    anim.SetIKHintPositionWeight(holder.ikHint, weight);
                    Vector3 hintPos = toWP(tP,holder.hintPos);
                    anim.SetIKHintPosition(holder.ikHint, hintPos);
                }
            }
        }

        public void InfluenceWeight(AvatarIKGoal ik, float t)
        {
            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    lf = t;
                    break;
                case AvatarIKGoal.LeftHand:
                    lh = t;
                    break;
                case AvatarIKGoal.RightFoot:
                    rf = t;
                    break;
                case AvatarIKGoal.RightHand:
                    rh = t;
                    break;
            }
        }  

        public Vector3 GetHipPos(Point p)
        {
            Vector3 hip = pointD.GetHip(p.pointType).hipPos;
            Vector3 targetP = p.transform.TransformPoint(hip);
            return targetP;
        }

        public Vector3 toWP(Point tp,Vector3 lp)
        {
            return  tp.transform.TransformPoint(lp);
        }

        PointType curPointT;

        public Vector3 CreateIKPos(Point tP, AvatarIKGoal ikGoal, PointType t)
        {
            Vector3 r = Vector3.zero;
            IKPositions ikPos = pointD.GetIKPos(ikGoal,t);
            r = toWP(tP, ikPos.ikPos);
            return r;
        }
    }
}
