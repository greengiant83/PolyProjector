using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionHandles : MonoBehaviour
{
    public Camera ProjectionCamera;
    public DistortionHandle[] Handles;
    public DistortionQuad ScreenQuad;
    
    Vector2[] sourcePoints = new Vector2[4];
    Vector2[] destinationPoints = new Vector2[4];

    void Start ()
    {		
	}
	
	void Update ()
    {		
	}

    public void UpdateCorners(DistortionHandle sender)
    {
        for (int i = 0; i < 4; i++)
        {
            sourcePoints[i] = Handles[i].OriginalHandlePosition;
            destinationPoints[i] = Handles[i].transform.localPosition;
        }

        for (int i = 0; i < 4; i++)
        {
            Handles[i].Corner.localPosition = DistortionHelper.TransferPoint(Handles[i].OriginalCornerPosition.x, Handles[i].OriginalCornerPosition.y, sourcePoints, destinationPoints);
        }
    }

    public void OnHandleDragComplete()
    {
        for(int i=0;i<4;i++)
        {
            Handles[i].StoreOriginalPositions();
        }
        Persistance.Instance.Save();
    }

    public void OnSourceHandlesChanged()
    {
        foreach(var handle in Handles)
        {
            var pos = ProjectionCamera.WorldToViewportPoint(handle.PreprojectionHandle.position);
            pos.z = 0;
            pos = ScreenQuad.TransferPoint(pos.x, pos.y);
            handle.transform.localPosition = pos;
            handle.StoreOriginalPositions();
        }
        Persistance.Instance.Save();
    }
}
