using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public int Step = 0;
    public bool TutorialFinished = false;
    public bool BlockSForever = false;

    private void Awake()
    {
        Instance = this;
    }
}
