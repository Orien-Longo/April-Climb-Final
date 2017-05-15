using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressToStart : MonoBehaviour
{
    bool canStart = false;

    void FixedUpdate()
    {
        if (Input.GetButtonDown("Jump"))
        {
            //Debug.Log("Start Game");
            canStart = true;            
            if (canStart)
            {
                WaitThenStart();
                SceneManager.LoadScene(1, LoadSceneMode.Single);
            }
        }
    }

    IEnumerator WaitThenStart()
    {
        
        yield return new WaitForSeconds(3);
        
        

    }
}
