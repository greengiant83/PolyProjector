using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionFace : MonoBehaviour
{
    public float Sensitivity = 5;
    public float MovementScalar = 0.001f;
    public Transform PreviewRotator;

    bool isDragging = false;
    Vector3 lastMousePos;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!AppController.Instance.CurrentState.IsCalibrating)
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                var rot = Input.mouseScrollDelta.y * Sensitivity;
                this.transform.Rotate(0, 0, rot, Space.Self);
                PreviewRotator.Rotate(0, 0, rot, Space.World);
            }

            if (Input.GetMouseButtonDown(0))
            {
                lastMousePos = Input.mousePosition;
                isDragging = true;
            }

            if (Input.GetMouseButton(0))
            {
                var delta = Input.mousePosition - lastMousePos;
                delta.x *= -1;
                transform.localPosition += delta * MovementScalar;
                lastMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
