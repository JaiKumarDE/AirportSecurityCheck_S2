using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject camera1;
    public GameObject camera2;
    public GameObject camera3;

    private GameObject[] cameras;
    private int currentCameraIndex = 0;

    void Start()
    {
        cameras = new GameObject[]
        {
            camera1,
            camera2,
            camera3
        };

        SetCamera(0);
    }

    void Update()
    {
        if (!TutorialManager.Instance.TutorialFinished &&
            TutorialManager.Instance.Step != 1 &&
            TutorialManager.Instance.Step != 4)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            HandleS();
        }
    }

    // 🔥 S LOGIK (TUTORIAL + LOOP)
    void HandleS()
    {
        if (!TutorialManager.Instance.TutorialFinished)
        {
            // Tutorial: normal weiter
            int next = currentCameraIndex + 1;

            if (next >= cameras.Length)
                next = 0;

            SetCamera(next);
            return;
        }

        // 🔥 NACH TUTORIAL: S nur 1 ↔ 2 LOOP
        if (currentCameraIndex == 1)
            SetCamera(2);
        else
            SetCamera(1);
    }

    // 🔥 BUTTONS
    public void SelectCamera(int index)
    {
        if (!TutorialManager.Instance.TutorialFinished)
            return;

        SetCamera(index);
    }

    // 🔥 SAFE SWITCH (KEIN BUG MEHR)
    void SetCamera(int index)
    {
        if (index < 0 || index >= cameras.Length)
            return;

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(i == index);
        }

        currentCameraIndex = index;

        Debug.Log("Aktive Kamera: " + cameras[index].name);
    }
}