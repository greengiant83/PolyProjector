using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homography : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Camera cam = GetComponent<Camera>();
        var matrix = cam.projectionMatrix;
        matrix = new Matrix4x4(
            new Vector4(1, 0, 0.1f, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 1)
        );
        cam.projectionMatrix = matrix;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
