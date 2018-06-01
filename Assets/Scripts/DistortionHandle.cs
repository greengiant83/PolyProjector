using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionHandle : MonoBehaviour
{
    public float Sensitivity = 0.1f;
    public DistortionHandles Parent;
    public Transform Corner;
    public Transform PreprojectionHandle;
    public Vector3 OriginalCornerPosition;
    public Vector3 OriginalHandlePosition;

    Vector3 lastMousePos;
    bool isDragging = false;

	void Start ()
    {
        StoreOriginalPositions();
	}
	
	void Update ()
    {
    }

    public void StoreOriginalPositions()
    {
        OriginalHandlePosition = transform.localPosition;
        OriginalCornerPosition = Corner.localPosition;
    }

    private void OnMouseDown()
    {
        lastMousePos = Input.mousePosition;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        var delta = Input.mousePosition - lastMousePos;
        delta.x *= -1;
        transform.localPosition += delta * Sensitivity;
        lastMousePos = Input.mousePosition;

        Parent.UpdateCorners(this);
    }

    private void OnMouseUp()
    {
        Parent.OnHandleDragComplete();
        isDragging = true;
    }
}
