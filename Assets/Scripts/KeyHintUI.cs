using TMPro;
using UnityEngine;

public class KeyHintUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI hintText;

    private void Start()
    {
        ShowDefault();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            hintText.text =
                "Mit s kannst du die Ansicht ändern. Drücke o um den koffer zu inspizieren";
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            hintText.text =
                "TOP. Mit p kannst du den Koffer aufmachen";
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            hintText.text =
                "Jetzt kannst du die items mit Maus draggen.";
        }
        /*
        if (Input.GetKeyDown(KeyCode.B))
        {
            hintText.text =
                "B gedrückt\n\nFörderband Richtung geändert";
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hintText.text =
                "SPACE\n\nBand Start / Stop";
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            hintText.text =
                "M\n\nBand schneller";
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            hintText.text =
                "N\n\nBand langsamer";
        }
    */
    }

    private void ShowDefault()
    {
        hintText.text =
@"Hallo, willkommen in ASC.
Drück V um einen Koffer zu generien";
    }
}