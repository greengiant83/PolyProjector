using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings
{
    public float Zoom;
    public Vector3 CameraPosition;
    public Vector3 CornerTL;
    public Vector3 CornerTR;
    public Vector3 CornerBR;
    public Vector3 CornerBL;
}

public class Persistance : MonoBehaviour
{
    public Camera ProjectionCamera;
    public Transform CornerTL;
    public Transform CornerTR;
    public Transform CornerBR;
    public Transform CornerBL;

    public static Persistance Instance;

    public void Save()
    {
        var settings = new Settings();
        settings.Zoom = ProjectionCamera.orthographicSize;
        settings.CameraPosition = ProjectionCamera.transform.localPosition;
        settings.CornerTL = CornerTL.localPosition;
        settings.CornerTR = CornerTR.localPosition;
        settings.CornerBR = CornerBR.localPosition;
        settings.CornerBL = CornerBL.localPosition;

        XmlSerializer serializer = new XmlSerializer(typeof(Settings));
        TextWriter writer = new StreamWriter("settings.xml");
        serializer.Serialize(writer, settings);
        writer.Close();
        writer.Dispose();
    }

    public void Load()
    {
        if (!File.Exists("settings.xml")) return;

        TextReader reader = new StreamReader("settings.xml");
        XmlSerializer serializer = new XmlSerializer(typeof(Settings));
        Settings settings = (Settings)serializer.Deserialize(reader);
        ProjectionCamera.orthographicSize = settings.Zoom;
        ProjectionCamera.transform.localPosition = settings.CameraPosition;
        CornerTL.localPosition = settings.CornerTL;
        CornerTR.localPosition = settings.CornerTR;
        CornerBR.localPosition = settings.CornerBR;
        CornerBL.localPosition = settings.CornerBL;

        FindObjectOfType<DistortionHandles>().OnSourceHandlesChanged();
    }

    public void Reset()
    {
        File.Delete("settings.xml");
        SceneManager.LoadScene(0);
    }

    private void Start()
    {
        Instance = this;
        Load();
    }
}
