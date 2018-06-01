using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 Axis = Vector3.up;
    public float Speed = 0.1f;
	
	void FixedUpdate ()
    {
        transform.Rotate(Axis, Speed, Space.World);
	}
}
