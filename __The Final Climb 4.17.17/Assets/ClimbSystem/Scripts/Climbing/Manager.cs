using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Climbing
{
    public class Manager : MonoBehaviour
    {
       // [HideInInspector]
        public List<Point> allPoints = new List<Point>();

        void Start()
        {
            PopulateAllPoints();
        }

        public void Init()
        {
            PopulateAllPoints();
        }

        void PopulateAllPoints()
        {
            Point[] allP = GetComponentsInChildren<Point>();

            foreach(Point p in allP)
            {
                if(!allPoints.Contains(p))
                    allPoints.Add(p);
            }
        }

        public Point ReturnClosest(Vector3 from)
        {
            Point retVal = null;

            float minDist = Mathf.Infinity;

            for (int i = 0; i < allPoints.Count; i++)
            {
                float dist = Vector3.Distance(allPoints[i].transform.position, from);

                if (allPoints[i].dismountPoint)
                    continue;

                if(dist < minDist)
                {
                    retVal = allPoints[i];
                    minDist = dist;
                }
            }

            return retVal;
        }
    }
}
