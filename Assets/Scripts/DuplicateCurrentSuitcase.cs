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
        if (!TutorialManager.Instance.TutorialFinished &&
            TutorialManager.Instance.Step != 5)
        {
            return;
        }

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

        if (currentCopySuitcase != null)
        {
            Destroy(currentCopySuitcase);
        }

        currentCopySuitcase = Instantiate(currentSuitcase);
        currentCopySuitcase.name = "koffer_Copy";

        currentCopySuitcase.transform.position = tableSpawnPoint.position;
        currentCopySuitcase.transform.rotation = currentSuitcase.transform.rotation;
        currentCopySuitcase.transform.localScale = currentSuitcase.transform.localScale;

        Rigidbody suitcaseRb = currentCopySuitcase.GetComponent<Rigidbody>();

        if (suitcaseRb != null)
        {
            suitcaseRb.useGravity = false;
            suitcaseRb.isKinematic = true;
            suitcaseRb.linearVelocity = Vector3.zero;
            suitcaseRb.angularVelocity = Vector3.zero;
        }

        SetupItems();

        SuitcaseJudge judge = FindFirstObjectByType<SuitcaseJudge>();
        if (judge != null)
        {
            judge.ResetForNewSuitcase();
        }

        Debug.Log("Koffer exakt kopiert!");
    }

    private void SetupItems()
    {
        Transform[] allChildren = currentCopySuitcase.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            if (child.gameObject == currentCopySuitcase)
                continue;

            if (!child.name.Contains("(Clone)"))
                continue;

            Collider col = child.GetComponent<Collider>();

            if (col == null)
            {
                MeshFilter mf = child.GetComponentInChildren<MeshFilter>();

                if (mf != null && mf.sharedMesh != null)
                {
                    MeshCollider mc = child.gameObject.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                    mc.convex = true;
                }
                else
                {
                    child.gameObject.AddComponent<BoxCollider>();
                }
            }

            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = child.gameObject.AddComponent<Rigidbody>();
            }

            rb.useGravity = false;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (child.GetComponent<DraggableItem>() == null)
            {
                child.gameObject.AddComponent<DraggableItem>();
            }

            // 🔥 WICHTIGER FIX: Layer setzen, damit Raycast funktioniert
            child.gameObject.layer = LayerMask.NameToLayer("Ziehen");
        }
    }

    public GameObject GetCurrentCopySuitcase()
    {
        return currentCopySuitcase;
    }
}
