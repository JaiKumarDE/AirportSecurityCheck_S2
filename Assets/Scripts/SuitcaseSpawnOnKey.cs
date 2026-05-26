using UnityEngine;

public class SuitcaseSpawnOnKey : MonoBehaviour
{
    [Header("Spawn Key")]
    [SerializeField] private KeyCode spawnKey = KeyCode.V;

    [Header("Suitcase Prefab")]
    [SerializeField] private GameObject suitcasePrefab;

    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;

    [Header("Spawn Rotation")]
    [SerializeField] private Vector3 spawnEulerRotation = new Vector3(-90f, 0f, 0f);

    [Header("Options")]
    [SerializeField] private bool destroyOldSuitcaseBeforeSpawn = false;

    [Header("Duplicate System")]
    [SerializeField] private DuplicateCurrentSuitcase duplicateSystem;

    private GameObject currentSuitcaseInstance;

    private void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnSuitcase();
        }
    }

    public void SpawnSuitcase()
    {
        if (suitcasePrefab == null)
        {
            Debug.LogWarning("Kein Suitcase Prefab gesetzt!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("Kein Spawn Point gesetzt!");
            return;
        }

        // Alten Koffer löschen
        if (destroyOldSuitcaseBeforeSpawn && currentSuitcaseInstance != null)
        {
            Destroy(currentSuitcaseInstance);
        }

        Quaternion rot = Quaternion.Euler(spawnEulerRotation);

        // Koffer spawnen
        currentSuitcaseInstance = Instantiate(
            suitcasePrefab,
            spawnPoint.position,
            rot
        );

        // Name setzen
        currentSuitcaseInstance.name = "koffer(Clone)";

        // ITEMS SPAWNEN
        SuitcaseRandomSpawner randomSpawner =
            currentSuitcaseInstance.GetComponent<SuitcaseRandomSpawner>();

        if (randomSpawner != null)
        {
            randomSpawner.ClearSpawned();
            randomSpawner.SpawnAllItems();
        }

        // GANZ WICHTIG:
        // aktuellen Koffer an Duplicate Script senden
        if (duplicateSystem != null)
        {
            duplicateSystem.SetCurrentSuitcase(currentSuitcaseInstance);

            Debug.Log("Aktueller Koffer gesetzt!");
        }
        else
        {
            Debug.LogWarning("Duplicate System fehlt!");
        }
    }
}