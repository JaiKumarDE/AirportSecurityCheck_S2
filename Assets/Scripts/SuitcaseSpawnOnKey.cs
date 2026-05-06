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
    [SerializeField] private bool spawnOnStart = false;

    private GameObject currentSuitcaseInstance;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnSuitcase();
        }
    }

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
            Debug.LogWarning("SuitcaseSpawnOnKey: Kein suitcasePrefab zugewiesen.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("SuitcaseSpawnOnKey: Kein spawnPoint zugewiesen.");
            return;
        }

        if (destroyOldSuitcaseBeforeSpawn && currentSuitcaseInstance != null)
        {
            Destroy(currentSuitcaseInstance);
        }

        Quaternion rot = Quaternion.Euler(spawnEulerRotation);

        GameObject newSuitcase = Instantiate(
            suitcasePrefab,
            spawnPoint.position,
            rot
        );

        currentSuitcaseInstance = newSuitcase;

        SuitcaseRandomSpawner randomSpawner = newSuitcase.GetComponent<SuitcaseRandomSpawner>();

        if (randomSpawner != null)
        {
            randomSpawner.ClearSpawned();
            randomSpawner.SpawnAllItems();
        }
        else
        {
            Debug.LogWarning("SuitcaseSpawnOnKey: SuitcaseRandomSpawner fehlt auf dem Prefab.");
        }
    }
}