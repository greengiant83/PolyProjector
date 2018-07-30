using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FaceVisualizer : MonoBehaviour
{
    public GameObject OnObjectFace;
    public GameObject ProjectionFace;
    public GameObject ObjectRotator;
    public TextMesh FaceLabel;
        
    Vector3 normal;

    void Start ()
    {
	}
	
	void Update ()
    {
        //Debug.DrawRay(PreviewArea.transform.position, normal, Color.red);
	}

    public void ShowFace()
    {
        var model = AppController.Instance.CurrentState.Model;
        if (model == null) return;

        var faceIndex = AppController.Instance.CurrentState.FaceIndex;
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var face = model.Faces[faceIndex];
        var indexMap = new Dictionary<int, int>();

        //Load up vertices and reference map
        foreach(var index in face.Indices)
        {
            vertices.Add(model.Vertices[index]);
            indexMap.Add(index, vertices.Count - 1);
        }

        //Add face triangles
        for (var i = 2; i < face.Indices.Length; i++)
        {
            triangles.Add(indexMap[face.Indices[i]]);
            triangles.Add(indexMap[face.Indices[i - 1]]);
            triangles.Add(indexMap[face.Indices[0]]);
        }

        //Rotate face to point foward
        var rot = Quaternion.FromToRotation(face.Normal, Vector3.forward);
        for(var i=0;i<vertices.Count;i++)
        {
            vertices[i] = rot * vertices[i];
        }

        //Move vertices so that it starts at its origin
        var offset = vertices[0] * -1;
        Vector3 centerPoint = new Vector3();
        for (var i = 0; i < vertices.Count; i++)
        {
            vertices[i] += offset;
            centerPoint += vertices[i];
        }
        centerPoint /= vertices.Count;

        //Update meshes 
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        ProjectionFace.GetComponent<MeshFilter>().sharedMesh = mesh;
        OnObjectFace.GetComponent<MeshFilter>().sharedMesh = mesh;

        //Update positions
        OnObjectFace.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, face.Normal);
        OnObjectFace.transform.localPosition = face.Origin + OnObjectFace.transform.forward * 0.0001f;

        ProjectionFace.transform.localPosition = mesh.bounds.center * -1;

        FaceLabel.transform.localPosition = centerPoint;

        //Update preview area rotation to show selected face
        var direction = ObjectRotator.transform.InverseTransformPoint(OnObjectFace.transform.position);
        direction = face.Normal;
        rot = Quaternion.FromToRotation(direction, Vector3.forward);
        ObjectRotator.GetComponent<RotateTo>().SetTargetRot(rot);

        var previewLabelName = "previewLabel" + AppController.Instance.CurrentState.FaceIndex;
        if (GameObject.Find(previewLabelName) == null)
        {
            var previewLabel = Instantiate<GameObject>(FaceLabel.gameObject);
            previewLabel.name = previewLabelName;
            previewLabel.transform.SetParent(OnObjectFace.transform, false);
            previewLabel.transform.SetParent(OnObjectFace.transform.parent, true);
            previewLabel.layer = LayerMask.NameToLayer("Distortion");
            previewLabel.GetComponent<TextMesh>().color = Color.white;
        }
    }
}
