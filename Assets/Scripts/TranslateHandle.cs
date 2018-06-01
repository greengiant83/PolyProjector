using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateHandle : MonoBehaviour
{
    public Transform SourceObject;
    public Camera Cam;

	void Start ()
    {		
	}
	
	void Update ()
    {
        var pos = Cam.WorldToViewportPoint(SourceObject.position);
        pos.z = 0;
        transform.localPosition = pos - new Vector3(0.5f, 0.5f, 0);
	}
}
