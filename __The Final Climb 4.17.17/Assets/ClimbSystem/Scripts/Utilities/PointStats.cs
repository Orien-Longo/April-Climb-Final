using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Climbing {
    public class PointStats : MonoBehaviour {

        public List<HipPos> hipPos = new List<HipPos>();

        public IKPositions GetIKPos(AvatarIKGoal goal, PointType t)
        {
            IKPositions r = null;

            HipPos h = GetHip(t);

            for (int i = 0; i < h.ikPos.Count; i++)
            {
                if (h.ikPos[i].ik == goal)
                {
                    r = h.ikPos[i];
                    break;
                }
            }

            return r;
        }

        public HipPos GetHip(PointType t)
        {
            HipPos r = null;
            for (int i = 0; i < hipPos.Count; i++)
            {
                if(hipPos[i].type == t)
                {
                    r = hipPos[i];
                    break;
                }
            }
            return r;
        }
    }

    [System.Serializable]
    public class HipPos
    {
        public PointType type;
        public Vector3 hipPos;
        public List<IKPositions> ikPos = new List<IKPositions>();
    }
}
