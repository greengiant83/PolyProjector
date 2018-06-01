using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using System.IO;

public class FaceData
{
    public int[] Indices;
    public Vector3 Normal;
    public Vector3 Origin;
}

public class ModelData
{
    public List<Vector3> Vertices = new List<Vector3>();
    public List<Vector3> RawNormals = new List<Vector3>();
    public List<Vector3> Normals = new List<Vector3>();
    public List<int> Triangles = new List<int>();
    public List<FaceData> Faces = new List<FaceData>();
}

public class ObjLoader : MonoBehaviour
{
    public GameObject PreviewObject;
    public TextMesh SizeLabel;

    ModelData model;
    
    public void SelectFile()
    {
        var extensions = new[] 
        {
            new ExtensionFilter("3d Models", "obj" ),
            new ExtensionFilter("All Files", "*" ),
        };

        var result = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (result != null && result.Length > 0)
        {
            loadFile(result[0]);
        }
        else Debug.Log("No file selected");
    }

    void loadFile(string filepath)
    {
        var lines = File.ReadAllLines(filepath);
        AppController.Instance.CurrentState.Model = model = new ModelData();

        foreach (string line in lines)
        {
            var words = line.Trim().Split(' ');
            if(words != null && words.Length > 0)
            {
                switch (words[0])
                {
                    case "v":
                        model.Vertices.Add(parseVector(words));
                        break;
                    case "vn":
                        model.RawNormals.Add(parseVector(words));
                        break;
                    case "f":
                        var vertIndices = new List<int>();
                        var normalIndices = new List<int>();
                        for (var w=1;w<words.Length;w++)
                        {
                            var word = words[w];
                            var tokens = word.Split('/');                            
                            vertIndices.Add(int.Parse(tokens[0]) - 1);
                            normalIndices.Add(int.Parse(tokens[2]) - 1);
                        }
                        var face = new FaceData();
                        face.Indices = vertIndices.ToArray();
                        face.Origin = model.Vertices[vertIndices[0]];
                        face.Normal = Vector3.Cross((model.Vertices[vertIndices[2]] - model.Vertices[vertIndices[0]]), (model.Vertices[vertIndices[1]] - model.Vertices[vertIndices[0]]));
                        model.Faces.Add(face);

                        for(var i=2;i<vertIndices.Count;i++)
                        {
                            //Winding is reversed to make the X axis mirroring jive
                            model.Triangles.Add(vertIndices[i]);
                            model.Triangles.Add(vertIndices[i - 1]);
                            model.Triangles.Add(vertIndices[0]);
                        }                        
                        break;
                }
            }
        }

        createGameObject(model);
    }

    void createGameObject(ModelData modelData)
    {
        var mesh = new Mesh();

        mesh.vertices = modelData.Vertices.ToArray();
        mesh.triangles = modelData.Triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
                
        var scalar = 1 / (Mathf.Max(mesh.bounds.extents.x, mesh.bounds.extents.y, mesh.bounds.extents.z) * 2);
        PreviewObject.transform.localScale = Vector3.one * scalar;
        PreviewObject.transform.localPosition = mesh.bounds.center * -1 * scalar;
        PreviewObject.GetComponent<MeshFilter>().sharedMesh = mesh;

        SizeLabel.text = string.Format("{0:0.00} x {1:0.00} x {2:0.00} Meters", mesh.bounds.extents.x * 2, mesh.bounds.extents.y * 2, mesh.bounds.extents.z);
    }

    Vector3 parseVector(string[] words)
    {
        return new Vector3(-1 * float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3])); //X axis is inverted to make exports from Blender match representation in Unity
    }
}
