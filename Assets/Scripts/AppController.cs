using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerState
{
    public bool IsModelVisible = true;
    public bool IsCalibrating = true;
    public ModelData Model;

    private int _faceIndex = 0;
    public int FaceIndex
    {
        get { return _faceIndex; }
        set
        {
            int newIndex = value;
            if (newIndex < 0) newIndex = Model.Faces.Count + newIndex;
            if (newIndex >= Model.Faces.Count) newIndex = newIndex % Model.Faces.Count;

            _faceIndex = newIndex;
        }
    }
}


public class AppController : MonoBehaviour
{
    public GameObject[] calibrationObjects;
    public static AppController Instance;
    public ControllerState CurrentState = new ControllerState();

    ObjLoader objLoader;
    FaceVisualizer faceVisualizer;
    GameObject previewArea;
    GameObject distortionArea;
    GameObject projectionFace;
    TextMesh faceLabel;    

	void Start ()
    {
        Instance = this;

        objLoader = FindObjectOfType<ObjLoader>();
        faceVisualizer = FindObjectOfType<FaceVisualizer>();
        previewArea = GameObject.Find("Preview Area");
        distortionArea = GameObject.Find("Distortion Area");
        projectionFace = GameObject.Find("Projection Face");
        faceLabel = GameObject.Find("Face Label").GetComponent<TextMesh>();

        syncUIFromState();
    }
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            objLoader.SelectFile();

            CurrentState.FaceIndex = 0;
            CurrentState.IsCalibrating = false;
            CurrentState.IsModelVisible = true;
            syncUIFromState();
        }

        if(Input.GetKeyDown(KeyCode.Comma))
        {
            CurrentState.FaceIndex--;
            syncUIFromState();
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            CurrentState.FaceIndex++;
            syncUIFromState();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            CurrentState.IsCalibrating = !CurrentState.IsCalibrating;
            syncUIFromState();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CurrentState.IsModelVisible = !CurrentState.IsModelVisible;
            syncUIFromState();
        }
    }

    void syncUIFromState()
    {
        foreach(var item in calibrationObjects)
        {
            item.SetActive(CurrentState.IsCalibrating);
        }

        previewArea.SetActive(!CurrentState.IsCalibrating && CurrentState.IsModelVisible);
        projectionFace.SetActive(!CurrentState.IsCalibrating);
        faceLabel.gameObject.SetActive(!CurrentState.IsCalibrating);

        faceVisualizer.ShowFace();
        faceLabel.text = CurrentState.FaceIndex.ToString("00");
    }
}
