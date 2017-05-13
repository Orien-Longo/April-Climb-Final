using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressToStart : MonoBehaviour {

	void Update () {
		if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Start Game");
            SceneManager.LoadScene("playground", LoadSceneMode.Single);
        }
	}

        
}
