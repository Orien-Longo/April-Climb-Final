using System;
using UnityEngine;


namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target, secondTarget;
        public bool triggered;
        //public Quaternion newLookAngle;
        public float speed = .1f;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);

        public void Awake()
        {
            triggered = false;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.smoothDeltaTime / speed);
            // transform.rotation = Quaternion.Slerp();

        }

        void Update()
        {
            if (transform.position.y <= 77)
            {
                transform.LookAt(target);
            }
            else
            {
                target = secondTarget;
            }
        }

        //private void OnCollisionEnter(Collision other)
        //{
        //    if (other.gameObject.CompareTag("Finish"))
        //    {
        //        triggered = true;
        //    }
        //}
    }
}
