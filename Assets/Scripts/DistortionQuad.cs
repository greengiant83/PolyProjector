using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionQuad : MonoBehaviour
{
    public Transform[] DestinationCorners;

    int rows = 50;
    int cols = 50;
    const float width = 16;
    const float height = 9;

    Mesh mesh;
    Vector3[] verts;

    //Starting in the top left and going clockwise
    Vector2[] sourceCorners = new Vector2[]
    {
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(0, 0),
    };

    //Starting in the top left and going clockwise
    Vector2[] destinationCorners = new Vector2[]
    {
        new Vector2(-width / 2, height / 2),
        new Vector2(width / 2, height / 2),
        new Vector2(width / 2, -height / 2),
        new Vector2(-width / 2, -height / 2),
    };

    void Start()
    {
        mesh = new Mesh();
        verts = new Vector3[rows * cols];
        updateVerts();
        initMesh();
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    void updateVerts()
    {
        for(int i=0;i<4;i++)
        {
            //sourceCorners[i] = SourceCorners[i].localPosition;
            destinationCorners[i] = DestinationCorners[i].localPosition;
        }
        
        for (int row = 0; row < rows; row++)
        {
            float y = row / (float)(rows - 1);
            for (int col = 0; col < cols; col++)
            {
                float x = col / (float)(cols - 1);
                verts[row * cols + col] = DistortionHelper.TransferPoint(x, y, sourceCorners, destinationCorners);
            }
        }

        mesh.vertices = verts;
    }

    public Vector3 TransferPoint(float x, float y)
    {
        return DistortionHelper.TransferPoint(x, y, sourceCorners, destinationCorners);
    }

    void initMesh()
    {
        mesh.vertices = verts;

        var triangles = new List<int>();
        var uvs = new Vector2[verts.Length];

        for (int row = 1; row < rows; row++)
        {
            for (int col = 1; col < cols; col++)
            {
                triangles.AddRange(new int[]
                {
                    getIndex(row-1, col-1), getIndex(row, col-1), getIndex(row-1, col),
                    getIndex(row-1, col), getIndex(row, col-1), getIndex(row, col)
                });
            }
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                uvs[getIndex(row, col)] = new Vector2((col / (float)(cols - 1)), row / (float)(rows - 1));
            }
        }

        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs;
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }


    

    int getIndex(int row, int col)
    {
        return row * cols + col;
    }

    void Update()
    {
        updateVerts();
    }
}

public static class DistortionHelper
{
    /*
        Source: https://stackoverflow.com/questions/12919398/perspective-transform-of-svg-paths-four-corner-distort
        and http://jsfiddle.net/xjHUk/209/

        transferPoint()

        Parameters:
         xI = x coordinate of point in original figure
         yI = y coordinate of point in original figure

        source = array of corner coordinates of original four-sided figure
        var source:Array = new Array();
        source[0] = new Point(0, 0);
        source[2] = new Point(200,100);

         destination = corner coordinates of destination (perspective distorted) figure
      Example of destination-array:

        var destination:Array = new Array();
        destination[n] = new Point(0, 0); //n=0...4
    */


    public static Vector3 TransferPoint(float xI, float yI, Vector2[] sourceCorners, Vector2[] destinationCorners)
    {
        float ADDING = 0.001f; // to avoid dividing by zero

        var xA = sourceCorners[0].x;
        var yA = sourceCorners[0].y;

        var xC = sourceCorners[2].x;
        var yC = sourceCorners[2].y;

        var xAu = destinationCorners[0].x;
        var yAu = destinationCorners[0].y;

        var xBu = destinationCorners[1].x;
        var yBu = destinationCorners[1].y;

        var xCu = destinationCorners[2].x;
        var yCu = destinationCorners[2].y;

        var xDu = destinationCorners[3].x;
        var yDu = destinationCorners[3].y;

        // Calcultations
        // if points are the same, have to add a ADDING to avoid dividing by zero
        if (xBu == xCu) xCu += ADDING;
        if (xAu == xDu) xDu += ADDING;
        if (xAu == xBu) xBu += ADDING;
        if (xDu == xCu) xCu += ADDING;
        var kBC = (yBu - yCu) / (xBu - xCu);
        var kAD = (yAu - yDu) / (xAu - xDu);
        var kAB = (yAu - yBu) / (xAu - xBu);
        var kDC = (yDu - yCu) / (xDu - xCu);

        if (kBC == kAD) kAD += ADDING;
        var xE = (kBC * xBu - kAD * xAu + yAu - yBu) / (kBC - kAD);
        var yE = kBC * (xE - xBu) + yBu;

        if (kAB == kDC) kDC += ADDING;
        var xF = (kAB * xBu - kDC * xCu + yCu - yBu) / (kAB - kDC);
        var yF = kAB * (xF - xBu) + yBu;

        if (xE == xF) xF += ADDING;
        var kEF = (yE - yF) / (xE - xF);

        if (kEF == kAB) kAB += ADDING;
        var xG = (kEF * xDu - kAB * xAu + yAu - yDu) / (kEF - kAB);
        var yG = kEF * (xG - xDu) + yDu;

        if (kEF == kBC) kBC += ADDING;
        var xH = (kEF * xDu - kBC * xBu + yBu - yDu) / (kEF - kBC);
        var yH = kEF * (xH - xDu) + yDu;

        var rG = (yC - yI) / (yC - yA);
        var rH = (xI - xA) / (xC - xA);

        var xJ = (xG - xDu) * rG + xDu;
        var yJ = (yG - yDu) * rG + yDu;

        var xK = (xH - xDu) * rH + xDu;
        var yK = (yH - yDu) * rH + yDu;

        if (xF == xJ) xJ += ADDING;
        if (xE == xK) xK += ADDING;
        var kJF = (yF - yJ) / (xF - xJ); //23
        var kKE = (yE - yK) / (xE - xK); //12

        if (kJF == kKE) kKE += ADDING;
        var xIu = (kJF * xF - kKE * xE + yE - yF) / (kJF - kKE);
        var yIu = kJF * (xIu - xJ) + yJ;

        var b = new Vector2(xIu, yIu);
        return b;
    }
}


//public class DistortionQuad : MonoBehaviour
//{
//    public Transform Center;
//    public Transform CornerTopLeft;
//    public Transform CornerTopRight;
//    public Transform CornerBottomRight;
//    public Transform CornerBottomLeft;

//    public DistortionHandle[] Handles;

//    Mesh mesh;
//    Vector3[] verts;

//    Vector2[] sourceCorners = new Vector2[]
//    {
//        new Vector2(0, 0),
//        new Vector2(1, 0),
//        new Vector2(1, 1),
//        new Vector2(0, 1)
//    };

//    Vector2[] distortedCorners = new Vector2[4];

//    int rows = 50;
//    int cols = 50;

//	void Start ()
//    {
//        mesh = new Mesh();
//        verts = new Vector3[rows * cols];
//        updateVerts();
//        initMesh();
//        transform.localScale = Vector3.one;
//        transform.localRotation = Quaternion.identity;
//    }

//    void lerpUpdateVerts()
//    {      
//        Vector3 left = CornerTopLeft.localPosition;
//        Vector3 right = CornerTopRight.localPosition;

//        for (int row = 0; row < rows; row++)
//        {
//            float y = row / (float)(rows-1);
//            left = Vector3.Lerp(CornerTopLeft.localPosition, CornerBottomLeft.localPosition, y);
//            right = Vector3.Lerp(CornerTopRight.localPosition, CornerBottomRight.localPosition, y);
//            for (int col = 0; col < cols;col++)
//            {
//                float x = col / (float)(cols-1);
//                verts[row * cols + col] = Vector3.Lerp(left, right, x);
//            }
//        }
//        mesh.vertices = verts;
//    }

//    void updateVerts()
//    {
//        distortedCorners[0] = CornerTopLeft.localPosition;
//        distortedCorners[1] = CornerTopRight.localPosition;
//        distortedCorners[2] = CornerBottomRight.localPosition;
//        distortedCorners[3] = CornerBottomLeft.localPosition;

//        for (int row = 0; row < rows; row++)
//        {
//            float y = row / (float)(rows - 1);
//            for (int col = 0; col < cols; col++)
//            {
//                float x = col / (float)(cols - 1);
//                verts[row * cols + col] = transferPoint(x, y, sourceCorners, distortedCorners);
//            }
//        }
//        mesh.vertices = verts;
//    }

//    void initMesh()
//    {
//        mesh.vertices = verts;

//        var triangles = new List<int>();
//        var uvs = new Vector2[verts.Length];

//        for (int row = 1; row < rows; row++)
//        {
//            for (int col = 1; col < cols; col++)
//            {
//                triangles.AddRange(new int[]
//                {
//                    getIndex(row-1, col), getIndex(row, col-1), getIndex(row-1, col-1),
//                    getIndex(row, col), getIndex(row, col-1), getIndex(row-1, col)
//                });
//            }
//        }

//        for (int row = 0; row < rows; row++)
//        {
//            for (int col = 0; col < cols; col++)
//            {
//                uvs[getIndex(row, col)] = new Vector2(col / (float)(cols-1), 1 - row / (float)(rows-1));
//            }
//        }

//        mesh.triangles = triangles.ToArray();
//        mesh.uv = uvs;
//        GetComponent<MeshFilter>().sharedMesh = mesh;
//    }


//    /*
//        Source: https://stackoverflow.com/questions/12919398/perspective-transform-of-svg-paths-four-corner-distort
//        and http://jsfiddle.net/xjHUk/209/

//        transferPoint()

//        Parameters:
//         xI = x coordinate of point in original figure
//         yI = y coordinate of point in original figure

//        source = array of corner coordinates of original four-sided figure
//        var source:Array = new Array();
//        source[0] = new Point(0, 0);
//        source[2] = new Point(200,100);

//         destination = corner coordinates of destination (perspective distorted) figure
//      Example of destination-array:

//        var destination:Array = new Array();
//        destination[n] = new Point(0, 0); //n=0...4
//    */

//    Vector3 transferPoint(float xI, float yI, Vector2[] source, Vector2[] destination)
//    {
//        float ADDING = 0.001f; // to avoid dividing by zero

//        var xA = source[0].x;
//        var yA = source[0].y;

//        var xC = source[2].x;
//        var yC = source[2].y;

//        var xAu = destination[0].x;
//        var yAu = destination[0].y;

//        var xBu = destination[1].x;
//        var yBu = destination[1].y;

//        var xCu = destination[2].x;
//        var yCu = destination[2].y;

//        var xDu = destination[3].x;
//        var yDu = destination[3].y;

//        // Calcultations
//        // if points are the same, have to add a ADDING to avoid dividing by zero
//        if (xBu == xCu) xCu += ADDING;
//        if (xAu == xDu) xDu += ADDING;
//        if (xAu == xBu) xBu += ADDING;
//        if (xDu == xCu) xCu += ADDING;
//        var kBC = (yBu - yCu) / (xBu - xCu);
//        var kAD = (yAu - yDu) / (xAu - xDu);
//        var kAB = (yAu - yBu) / (xAu - xBu);
//        var kDC = (yDu - yCu) / (xDu - xCu);

//        if (kBC == kAD) kAD += ADDING;
//        var xE = (kBC * xBu - kAD * xAu + yAu - yBu) / (kBC - kAD);
//        var yE = kBC * (xE - xBu) + yBu;

//        if (kAB == kDC) kDC += ADDING;
//        var xF = (kAB * xBu - kDC * xCu + yCu - yBu) / (kAB - kDC);
//        var yF = kAB * (xF - xBu) + yBu;

//        if (xE == xF) xF += ADDING;
//        var kEF = (yE - yF) / (xE - xF);

//        if (kEF == kAB) kAB += ADDING;
//        var xG = (kEF * xDu - kAB * xAu + yAu - yDu) / (kEF - kAB);
//        var yG = kEF * (xG - xDu) + yDu;

//        if (kEF == kBC) kBC += ADDING;
//        var xH = (kEF * xDu - kBC * xBu + yBu - yDu) / (kEF - kBC);
//        var yH = kEF * (xH - xDu) + yDu;

//        var rG = (yC - yI) / (yC - yA);
//        var rH = (xI - xA) / (xC - xA);

//        var xJ = (xG - xDu) * rG + xDu;
//        var yJ = (yG - yDu) * rG + yDu;

//        var xK = (xH - xDu) * rH + xDu;
//        var yK = (yH - yDu) * rH + yDu;

//        if (xF == xJ) xJ += ADDING;
//        if (xE == xK) xK += ADDING;
//        var kJF = (yF - yJ) / (xF - xJ); //23
//        var kKE = (yE - yK) / (xE - xK); //12

//        //var xKE;
//        if (kJF == kKE) kKE += ADDING;
//        var xIu = (kJF * xF - kKE * xE + yE - yF) / (kJF - kKE);
//        var yIu = kJF * (xIu - xJ) + yJ;

//        //var b = new Vector2(Mathf.Round(xIu), Mathf.Round(yIu));
//        var b = new Vector2(xIu, yIu);
//        return b;
//    }

//    int getIndex(int row, int col)
//    {
//        return row * cols + col;
//    }

//	void Update ()
//    {
//        Center.localPosition = Vector3.Lerp(
//            Vector3.Lerp(Handles[0].transform.position, Handles[1].transform.position, 0.5f),
//            Vector3.Lerp(Handles[2].transform.position, Handles[3].transform.position, 0.5f),
//            0.5f);

//        foreach (var handle in Handles)
//        {
//            var centerVector = handle.transform.localPosition - Center.position;
//            var centerDistance = centerVector.magnitude;
//            centerVector.Normalize();
//            handle.Corner.localPosition = handle.transform.localPosition + centerVector * handle.CornerDistance * centerDistance;
//        }

//        updateVerts();
//	}
//}
