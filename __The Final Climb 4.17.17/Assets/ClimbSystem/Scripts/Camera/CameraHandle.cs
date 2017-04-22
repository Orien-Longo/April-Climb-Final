using UnityEngine;
using System.Collections;

namespace Controller
{
    public class CameraHandle : MonoBehaviour
    {
        static public CameraHandle singleton;
        void Awake()
        {
            singleton = this;
        }

        public Transform target;
        Transform pivot;
        Transform camTrans;

        public float lerpSpeed = 5;

        float turnSpeed = 1.5f;
        float turnSmoothing = .1f;
        float tiltAngle;
        float tiltMax = 75f;
        float tiltMin = 45f;
        float smoothX;
        float smoothY;
        float smoothXvelocity = 0;
        float smoothYvelocity = 0;

        float lookAngle;

        float defaultZ;
        LayerMask ignoreLayers;

        void Start()
        {
            transform.position = target.position;
            pivot = transform.GetChild(0);
            camTrans = Camera.main.transform;
            defaultZ = camTrans.localPosition.z;

            ignoreLayers = ~(1 << 3 | 1 << 9 | 1 << 11);
        }

        void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * lerpSpeed);

            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            if(turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, x, ref smoothXvelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, y, ref smoothYvelocity, turnSmoothing);
            }
            else
            {
                smoothX = x;
                smoothY = y;
            }

            lookAngle += smoothX * turnSpeed;

            if (lookAngle > 360)
                lookAngle = 0;
            if (lookAngle < -360)
                lookAngle = 0;

            transform.rotation = Quaternion.Euler(0f, lookAngle, 0);

            tiltAngle -= smoothY * turnSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, -tiltMin, tiltMax);

            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

            float actualZ = defaultZ;
            CameraCollision(defaultZ, ref actualZ);
            Vector3 targetP = camTrans.localPosition;
            targetP.z = Mathf.Lerp(targetP.z, actualZ, Time.deltaTime * 5);
            camTrans.localPosition = targetP;
        }

        void CameraCollision(float targetZ, ref float actualZ)
        {
            float step = Mathf.Abs(targetZ);
            int stepCount = 2;
            float stepIncremental = step / stepCount;

            RaycastHit hit;
            Vector3 origin = pivot.position;
            Vector3 direction = -pivot.forward;
            Debug.DrawRay(origin, direction * step, Color.blue);

            if (Physics.Raycast(origin, direction, out hit, step, ignoreLayers))
            {
                float distance = Vector3.Distance(hit.point, origin);
                actualZ = -(distance / 2);
            }
            else
            {
                for (int s = 1; s < stepCount + 1; s++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 dir = Vector3.zero;
                        Vector3 secondOrigin = origin + (direction * s) * stepIncremental;
                        //Vector3 secondOrigin = origin + direction * step;

                        switch (i)
                        {
                            case 0:
                                dir = camTrans.right;
                                break;
                            case 1:
                                dir = -camTrans.right;
                                break;
                            case 2:
                                dir = camTrans.up;
                                break;
                            case 3:
                                dir = -camTrans.up;
                                break;
                            default:
                                break;
                        }

                        Debug.DrawRay(secondOrigin, dir * 1, Color.red);
                        if (Physics.Raycast(secondOrigin, dir, out hit, 1, ignoreLayers))
                        {
                            float distance = Vector3.Distance(secondOrigin, origin);
                            actualZ = -(distance / 2);
                            return;
                        }
                    }
                }
            }
        }
    }
}
