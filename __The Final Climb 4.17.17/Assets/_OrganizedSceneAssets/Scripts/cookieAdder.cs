using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cookieAdder : MonoBehaviour
{
    //public Texture SunCookie;
    float cookie, cookieRate;

    // Use this for initialization
    void Start()
    {

        cookie = 107.7f;

        cookieRate = .5f;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Light>().cookieSize = cookie;
        if (cookie < 402.6f)
        {


            cookie += cookieRate;



            //Debug.Log(gameObject.GetComponent<Light>().cookieSize);
        }
        else
        {
            cookie = 107.7f;
        }
        Debug.Log(gameObject.GetComponent<Light>().cookieSize);
    }
}
