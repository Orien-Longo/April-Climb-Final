using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Climbing
{
    [System.Serializable]
    public class Point : MonoBehaviour
    {
        public PointType pointType;
        public bool dismountPoint;
        public bool doubleSided;
    }

    [System.Serializable]
    public class IKPositions
    {
        public AvatarIKGoal ik;
        public Vector3 ikPos;
        public Vector3 ikRot;
        public Vector3 hintPos;
        public AvatarIKHint ikHint;
        public bool hasHint;
    }

    [System.Serializable]
    public class Neighbour
    {
        public Vector3 direction;
        public Point target;
        public ConnectionType cType;
        public bool customConnection;
    }

    public enum ConnectionType
    {
        inBetween,
        direct,
        dismount,
        fall,
        jumpBack,
        jumpBack_onManager,
        hanging_turn_around,
        hanging_jump_forward,
        hanging_jump_air,
        corner_out,corner_in

    }

    public enum PointType
    {
        braced,
        hanging,
        ladder,
        column
    }
}
