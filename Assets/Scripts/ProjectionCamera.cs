using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionCamera : MonoBehaviour
{
    public float MovementScalar = 0.01f;

    Camera cam;
    Vector3 lastMousePos;
    bool isDragging;

	void Start ()
    {
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        return;
        if (AppController.Instance.CurrentState.IsCalibrating)
        {
            cam.orthographicSize += Input.mouseScrollDelta.y * .1f;

            if (Input.GetMouseButtonDown(0))
            {
                lastMousePos = Input.mousePosition;
                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                var delta = Input.mousePosition - lastMousePos;
                delta.y *= -1;
                transform.localPosition += delta * MovementScalar * cam.orthographicSize;
                lastMousePos = Input.mousePosition;
            }
        }
    }
}
