using UnityEngine;
using System.Collections;

namespace Climbing
{
    public class ClimbEvents : MonoBehaviour
    {
        ClimbBehaviour cb;

        void Start()
        {
            cb = transform.root.GetComponentInChildren<ClimbBehaviour>();
        }

        public void EnableRootMovement(float t)
        {
            StartCoroutine(Enable(t));
        }

        IEnumerator Enable(float t)
        {
            yield return new WaitForSeconds(t);
            cb.enableRootMovement = true;
        }
    }
}
