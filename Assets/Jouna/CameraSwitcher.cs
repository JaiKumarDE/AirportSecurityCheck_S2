using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    private bool usingFirstCamera = true;

    void Start()
    {
        // Startzustand
        camera1.enabled = true;
        camera2.enabled = false;
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
        usingFirstCamera = !usingFirstCamera;

        camera1.enabled = usingFirstCamera;
        camera2.enabled = !usingFirstCamera;
    }
}