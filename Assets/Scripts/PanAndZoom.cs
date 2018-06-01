using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanAndZoom : MonoBehaviour
{
    public DistortionHandles Handles;
    public Camera Cam;
    public float MovementScalar = 0.01f;
    public float ZoomScalar = 0.01f;

    bool isDragging = false;
    Vector3 lastMousePos;

	void Start ()
    {		
	}
	
	void Update ()
    {
        if (AppController.Instance.CurrentState.IsCalibrating && Input.mouseScrollDelta.y != 0)
        {
            Cam.orthographicSize += Input.mouseScrollDelta.y * ZoomScalar;
            Handles.OnSourceHandlesChanged();
        }
    }

    private void OnMouseDown()
    {
        if (!AppController.Instance.CurrentState.IsCalibrating) return;
        lastMousePos = Input.mousePosition;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            var delta = Input.mousePosition - lastMousePos;
            delta.y *= -1;
            Cam.transform.localPosition += delta * MovementScalar * Cam.orthographicSize;
            lastMousePos = Input.mousePosition;
            Handles.OnSourceHandlesChanged();
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }
}
