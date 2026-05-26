using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public Camera camera1;
    public Camera camera2;
    public Camera camera3;

    private int currentCameraIndex = 0;

    private Camera[] cameras;

    void Start()
    {
        // Array erstellen
        cameras = new Camera[]
        {
            camera1,
            camera2,
            camera3
        };

        // Alle Kameras ausschalten
        foreach (Camera cam in cameras)
        {
            if (cam != null)
            {
                cam.enabled = false;
            }
        }

        // Erste Kamera aktivieren
        if (cameras.Length > 0 && cameras[0] != null)
        {
            cameras[0].enabled = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        // Aktuelle Kamera ausschalten
        cameras[currentCameraIndex].enabled = false;

        // Zur nächsten Kamera wechseln
        currentCameraIndex++;

        // Wieder von vorne anfangen
        if (currentCameraIndex >= cameras.Length)
        {
            currentCameraIndex = 0;
        }

        // Neue Kamera aktivieren
        cameras[currentCameraIndex].enabled = true;

        Debug.Log("Aktive Kamera: " + cameras[currentCameraIndex].name);
    }
}