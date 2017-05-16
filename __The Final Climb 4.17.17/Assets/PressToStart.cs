using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressToStart : MonoBehaviour
{
    static bool canStart;

    private void Awake()
    {
         canStart = false;
    }

    void Update()
    {
        if (LerpUpandDown.reachedEnd)
        {
            BeginGame();
        }
        
        if (canStart)
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
            Debug.Log("Start Game");

        }
    }

    IEnumerator WaitThenStart()
    {
        yield return true;
        yield return new WaitForSeconds(3); 
        
        canStart = true;
    }

    void BeginGame()
    {
        if (Input.GetButtonDown("Jump"))
        {
            
            WaitThenStart();

        }
        
    }
}
