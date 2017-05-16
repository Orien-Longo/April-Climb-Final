using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpUpandDown : MonoBehaviour
{

    public float lerpSpeed, yPos;
    public Vector3 newPosition;
    public static bool reachedEnd;
    public float waitTime = 2f;

    void Awake()
    {
        reachedEnd = false;
        lerpSpeed = 2f;
        newPosition = transform.position;

    }

    void FixedUpdate()
    {

        LerpSwitch();

    }

    void LerpSwitch()
    {

        Vector3 _startV = new Vector3(0, 3.5f, 3.58f);
        Vector3 _endV = new Vector3(0, 0, 3.58f);

        if (!reachedEnd)
        {
            newPosition = _endV;


        }
        if (reachedEnd)
        {

            newPosition = _startV;


        }
        transform.position = Vector3.LerpUnclamped(transform.position, newPosition, Time.deltaTime * lerpSpeed);

        if (Input.GetButtonDown("Jump"))
        {
            reachedEnd = true;

        }
    }

}
