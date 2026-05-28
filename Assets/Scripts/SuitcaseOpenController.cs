using UnityEngine;

public class SuitcaseOpenController : MonoBehaviour
{
    [Header("Taste")]
    [SerializeField] private KeyCode openKey = KeyCode.P;

    [Header("Duplicate System")]
    [SerializeField] private DuplicateCurrentSuitcase duplicateSystem;

    [Header("Name vom Deckel Objekt")]
    [SerializeField] private string lidObjectName = "Lid";

    [Header("Öffnungsrotation")]
    [SerializeField]
    private Vector3 openRotation =
        new Vector3(-120f, 0f, 0f);

    [Header("Geschwindigkeit")]
    [SerializeField]
    private float openSpeed = 4f;

    private Transform currentLid;

    private bool isOpen;

    private Quaternion closedRotation;

    private Quaternion targetRotation;

    // =====================================================

    private void Update()
    {
        // P gedrückt
        if (Input.GetKeyDown(openKey))
        {
            ToggleSuitcase();
        }

        // Animation
        if (currentLid != null)
        {
            currentLid.localRotation =
                Quaternion.Lerp(
                    currentLid.localRotation,
                    targetRotation,
                    Time.deltaTime * openSpeed
                );
        }
    }

    // =====================================================

    private void ToggleSuitcase()
    {
        GameObject copySuitcase =
            duplicateSystem.GetCurrentCopySuitcase();

        if (copySuitcase == null)
        {
            Debug.LogWarning(
                "Kein koffer_Copy vorhanden!"
            );

            return;
        }

        // DECKEL SUCHEN
        currentLid =
            FindChildByName(
                copySuitcase.transform,
                lidObjectName
            );

        if (currentLid == null)
        {
            Debug.LogWarning(
                "Deckel nicht gefunden! Name prüfen: "
                + lidObjectName
            );

            return;
        }

        closedRotation = Quaternion.identity;

        isOpen = !isOpen;

        // Zielrotation
        if (isOpen)
        {
            targetRotation =
                Quaternion.Euler(openRotation);
        }
        else
        {
            targetRotation = closedRotation;
        }

        Debug.Log("Koffer geöffnet/geschlossen!");
    }

    // =====================================================

    private Transform FindChildByName(
        Transform parent,
        string searchName
    )
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(searchName))
            {
                return child;
            }

            Transform result =
                FindChildByName(
                    child,
                    searchName
                );

            if (result != null)
                return result;
        }

        return null;
    }
}