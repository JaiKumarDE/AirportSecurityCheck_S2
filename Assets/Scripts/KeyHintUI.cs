using TMPro;
using UnityEngine;

public class KeyHintUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private GameObject uiPanel;

    private bool waitingAfterP = false;

    private void Start()
    {
        uiPanel.SetActive(true);

        hintText.text =
@"Hallo, willkommen bei ASC!

Drücke ENTER um zu starten.";
    }

    private void Update()
    {
        switch (TutorialManager.Instance.Step)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    TutorialManager.Instance.Step = 1;
                    hintText.text = "Drücke S um die Kamera zu wechseln.";
                }
                break;

            case 1:
                if (Input.GetKeyDown(KeyCode.S))
                {
                    TutorialManager.Instance.Step = 2;
                    hintText.text = "Drücke V um einen Koffer zu spawnen.";
                }
                break;

            case 2:
                if (Input.GetKeyDown(KeyCode.V))
                {
                    TutorialManager.Instance.Step = 3;
                    hintText.text = "Drücke LEERTASTE um das Laufband zu stoppen.";
                }
                break;

            // 🔥 SPACE STEP (IMPORTANT)
            case 3:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TutorialManager.Instance.Step = 4;
                    hintText.text = "Drücke erneut S um die Kamera zu wechseln.";
                }
                break;

            case 4:
                if (Input.GetKeyDown(KeyCode.S))
                {
                    TutorialManager.Instance.Step = 5;
                    hintText.text = "Drücke O um den Koffer zu inspizieren.";
                }
                break;

            case 5:
                if (Input.GetKeyDown(KeyCode.O))
                {
                    TutorialManager.Instance.Step = 6;
                    hintText.text = "Drücke P um den Koffer zu öffnen.";
                }
                break;

            // 🔥 P WITH DELAY (FIXED SAFE VERSION)
            case 6:
                if (Input.GetKeyDown(KeyCode.P) && !waitingAfterP)
                {
                    waitingAfterP = true;

                    hintText.text = "Koffer wird geöffnet..";

                    StartCoroutine(PauseAfterP());
                }
                break;

            case 7:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    TutorialManager.Instance.Step = 8;
                    hintText.text = "Tutorial abgeschlossen!\nDrücke ENTER um zu beenden.";
                }
                break;

            case 8:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    hintText.text = "";

                    if (uiPanel != null)
                        uiPanel.SetActive(false);

                    TutorialManager.Instance.TutorialFinished = true;
                    TutorialManager.Instance.BlockSForever = true;
                }
                break;
        }
    }

    // 🔥 FIXED DELAY
    private System.Collections.IEnumerator PauseAfterP()
    {
        yield return new WaitForSeconds(3f);

        waitingAfterP = false;

        TutorialManager.Instance.Step = 7;

        hintText.text =
@"Super! Jetzt kannst du die Items bewegen.

Drücke ENTER um fortzufahren.";
    }
}