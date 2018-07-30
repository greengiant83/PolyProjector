using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTo : MonoBehaviour
{
    public float Speed = 0.01f;

    Quaternion targetRot;
    bool isActive = false;

	void Start ()
    {		
	}
	
	void Update ()
    {		
        if(isActive)
        {
            this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, targetRot, Speed);
            if(Quaternion.Angle(this.transform.localRotation, targetRot) < 0.1f)
            {
                Debug.Log("Rot done");
                isActive = false;
            }
        }
	}

    public void SetTargetRot(Quaternion targetRot)
    {
        this.targetRot = targetRot;
        isActive = true;
    }
}
