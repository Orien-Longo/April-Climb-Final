using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

    public Transform target1;
    public Transform target2;

    public float speed = 1;
    public AnimationCurve curve;
    float t;

	void FixedUpdate () {

        t += Time.deltaTime*speed;
        if(t > 1)
        {
            t = 0;
        }

        float ct = curve.Evaluate(t);

        Vector3 targetPosition = Vector3.Lerp(target1.position, target2.position, ct);
        transform.position = targetPosition;
	
	}
}
