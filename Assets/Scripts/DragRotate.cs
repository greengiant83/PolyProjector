using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragRotate : MonoBehaviour
{
    Vector3 lastMousePos;
    
    private void OnMouseDown()
    {
        lastMousePos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        var mouseDelta = Input.mousePosition - lastMousePos;
        var rot = new Vector3(-mouseDelta.y, -mouseDelta.x, 0);
        transform.Rotate(rot, Space.World);
        

        lastMousePos = Input.mousePosition;
    }
}
