#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using Climbing;

[ExecuteInEditMode]
public class AnimatorInEditor : MonoBehaviour {

    public Animator anim;
    public PointSetup_Utility ps;
    public bool disableLegs;

    public bool placeOnTransform;
    public Transform targetTransform;

    [Range(0,1)]
    public float stance;
    public bool enableRotation;
    public bool enableRotationFeet;

    void Update () {
    
        if(placeOnTransform)
        {
            anim.transform.position = targetTransform.position;
            anim.transform.position += new Vector3(0, -0.86f, 0);
            placeOnTransform = false;
        }

        if(anim)
        {
            anim.SetFloat("Stance", stance);
            anim.Update(0);
        }
	
	}

    void OnAnimatorIK()
    {
        if(anim)
        {
            if (ps == null)
                return;

            foreach (PointSetup_Utility.HelpersIK ik in ps.helpersIK)
            {
                anim.SetIKPositionWeight(ik.ikGoal, 1);
                anim.SetIKPosition(ik.ikGoal, ik.ikTarget.transform.position);

                if(enableRotation)
                {
                    if (ik.ikGoal == AvatarIKGoal.LeftHand || ik.ikGoal == AvatarIKGoal.RightHand)
                    {
                        anim.SetIKRotationWeight(ik.ikGoal, 1);
                        anim.SetIKRotation(ik.ikGoal, ik.ikTarget.transform.rotation);
                    }
                    else
                    {
                        if(enableRotationFeet)
                        {
                            anim.SetIKRotationWeight(ik.ikGoal, 1);
                            anim.SetIKRotation(ik.ikGoal, ik.ikTarget.transform.rotation);
                        }
                    }
                }
                else
                {
                    anim.SetIKRotationWeight(ik.ikGoal, 0);
                }                 

                if(disableLegs)
                {
                    if(ik.ikGoal == AvatarIKGoal.LeftFoot || ik.ikGoal == AvatarIKGoal.RightFoot)
                    {
                        anim.SetIKPositionWeight(ik.ikGoal, 0);
                    }
                }

                if(ik.targetHint)
                {
                    anim.SetIKHintPositionWeight(ik.ikHint, 1);
                    anim.SetIKHintPosition(ik.ikHint, ik.targetHint.transform.position);
                }
            }

        }
    }
}
#endif
