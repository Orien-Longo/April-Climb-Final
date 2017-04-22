#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Climbing;

[ExecuteInEditMode]
public class PointSetup_Utility : MonoBehaviour
{
    public PointType curType;
    public bool savePointSetup;
    public List<HelpersIK> helpersIK = new List<HelpersIK>();
    public Transform hipsHelper;
    [Header("Assign this if it's null")]
    public PointStats pointStatsToUpdate;

    void Update()
    {
        if (savePointSetup)
        {
            CreatePoint();
            savePointSetup = false;
        }
    }

    void CreatePoint()
    {
        if (pointStatsToUpdate == null)
        {
            GameObject go = new GameObject();
            go.AddComponent<PointStats>();
            pointStatsToUpdate = go.GetComponent<PointStats>();
            go.name = "PointStats";
        }

        HipPos cur = pointStatsToUpdate.GetHip(curType);

        if (cur == null)
        {
            HipPos hp = new HipPos();          
            hp.type = curType;
            cur = hp;
        }

        cur.hipPos = hipsHelper.localPosition;

        //Store IK Positions
        if (cur.ikPos.Count == 4)
        {
            for (int i = 0; i < 4; i++)
            {
                HelpersIK hik = helpersIK[i];
                IKPositions ik = cur.ikPos[i];
                UpdateVariables(ik, hik, cur);
            }
        }
        else
        {
            for (int i = 0; i < helpersIK.Count; i++)
            {
                HelpersIK hik = helpersIK[i];
                if (hik.ikTarget == null)
                    continue;
                IKPositions ik = new IKPositions();
                UpdateVariables(ik, hik, cur);
                cur.ikPos.Add(ik);
            }
        }

        if (!pointStatsToUpdate.hipPos.Contains(cur))
        {
            pointStatsToUpdate.hipPos.Add(cur);
        }

        Debug.Log("Point Stats saved, don't forget to apply it to the controller!");
    }

    void UpdateVariables(IKPositions ik, HelpersIK hik, HipPos cur)
    {
        ik.ikPos = hik.ikTarget.localPosition;
        ik.ik = hik.ikGoal;

        if (hik.targetHint)
        {
            ik.hasHint = true;
            ik.hintPos = hik.targetHint.localPosition;
            ik.ikHint = hik.ikHint;
        }

        ik.ikRot = hik.ikTarget.localEulerAngles;
    }

    [System.Serializable]
    public class HelpersIK
    {
        public AvatarIKGoal ikGoal;
        public Transform ikTarget;
        public Transform targetHint;
        public AvatarIKHint ikHint;
    }
}

#endif