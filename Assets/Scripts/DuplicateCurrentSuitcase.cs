using UnityEngine;

public class DuplicateCurrentSuitcase : MonoBehaviour
{
    [Header("Taste zum Kopieren")]
    [SerializeField] private KeyCode duplicateKey = KeyCode.O;

    [Header("Spawn Position")]
    [SerializeField] private Transform tableSpawnPoint;

    private GameObject currentSuitcase;
    private GameObject currentCopySuitcase;

    private void Update()
    {
        if (Input.GetKeyDown(duplicateKey))
        {
            DuplicateSuitcase();
        }
    }

    public void SetCurrentSuitcase(GameObject suitcase)
    {
        currentSuitcase = suitcase;
    }

    private void DuplicateSuitcase()
    {
        if (currentSuitcase == null)
        {
            Debug.LogWarning("Kein aktueller Koffer vorhanden!");
            return;
        }

        // ALTEN COPY KOFFER LÖSCHEN
        if (currentCopySuitcase != null)
        {
            Destroy(currentCopySuitcase);
        }

        // KOPIEREN
        currentCopySuitcase = Instantiate(currentSuitcase);

        currentCopySuitcase.name = "koffer_Copy";

        currentCopySuitcase.transform.position = tableSpawnPoint.position;

        currentCopySuitcase.transform.rotation =
            currentSuitcase.transform.rotation;

        currentCopySuitcase.transform.localScale =
            currentSuitcase.transform.localScale;

        // Rigidbody resetten
        Rigidbody rb = currentCopySuitcase.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // ALLE ITEMS DRAGGABLE MACHEN
        Rigidbody[] rigidbodies =
            currentCopySuitcase.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody childRb in rigidbodies)
        {
            if (childRb.gameObject == currentCopySuitcase)
                continue;

            if (childRb.GetComponent<DraggableItem>() == null)
            {
                childRb.gameObject.AddComponent<DraggableItem>();
            }
        }

        Debug.Log("Koffer exakt kopiert!");
    }

    public GameObject GetCurrentCopySuitcase()
    {
        return currentCopySuitcase;
    }
}