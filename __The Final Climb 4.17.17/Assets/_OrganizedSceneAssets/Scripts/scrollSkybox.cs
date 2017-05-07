using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class scrollSkybox : MonoBehaviour {

    public Skybox airCube;
    float airCubeRot = 0;
    float rotationSpeed = 10f;

	// Use this for initialization
	void Start () {
        airCube = FindObjectOfType<Skybox>();
        //airCube = GetComponent<Skybox>().GetComponent<;
	}
	
	
	void Update () {
        //airCubeRot = airCube.material.GetFloat("Rotation");
        if (airCubeRot < 360f)
        {
            airCubeRot += rotationSpeed * 10f;
        }
        else
        {
            airCubeRot = 0f;
        }
        Debug.Log("airCubeRot");
    }
}
