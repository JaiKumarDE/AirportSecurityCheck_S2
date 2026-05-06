using UnityEngine;
using System.Collections.Generic;

public class SuitcaseRandomSpawner : MonoBehaviour
{
    [Header("Spawn Area (BoxCollider)")]
    [SerializeField] private BoxCollider spawnArea;

    [Header("Fixed Items (werden immer zuerst gespawnt)")]
    [SerializeField] private List<GameObject> fixedPrefabs;

    [Header("Random Items")]
    [SerializeField] private List<GameObject> randomPrefabs;

    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 0.01f;
    [SerializeField] private int maxItems = 200;

    [Header("Rotation")]
    [SerializeField] private bool randomYRotation = true;

    [Header("Scaling")]
    [SerializeField] private float fitMultiplier = 0.9f;
    [SerializeField] private float minScale = 0.2f;
    [SerializeField] private float maxScale = 1.0f;

    private List<GameObject> spawnedItems = new List<GameObject>();
    private List<Bounds> placedBounds = new List<Bounds>();

    public void SpawnAllItems()
    {
        if (spawnArea == null)
        {
            Debug.LogWarning("SpawnArea fehlt!");
            return;
        }

        ClearSpawned();

        Bounds area = spawnArea.bounds;

        // 🟢 1. FIXE OBJEKTE ZUERST
        foreach (GameObject prefab in fixedPrefabs)
        {
            TryPlaceFixed(prefab);
        }

        // 🔵 2. GRID FÜR RANDOM OBJEKTE
        for (float y = area.min.y; y < area.max.y; y += cellSize)
        {
            for (float x = area.min.x; x < area.max.x; x += cellSize)
            {
                for (float z = area.min.z; z < area.max.z; z += cellSize)
                {
                    if (spawnedItems.Count >= maxItems)
                        return;

                    TryPlaceRandom(new Vector3(x, y, z));
                }
            }
        }
    }

    // ================= FIXE ITEMS =================

    void TryPlaceFixed(GameObject prefab)
    {
        // Versucht mehrere Positionen für fixe Objekte
        for (int i = 0; i < 50; i++)
        {
            Vector3 pos = GetRandomPointInBounds();

            Quaternion rot = randomYRotation
                ? Quaternion.Euler(0, Random.Range(0, 360), 0)
                : Quaternion.identity;

            GameObject obj = Instantiate(prefab, pos, rot, transform);

            ScaleToFit(obj);

            Bounds bounds = GetObjectBounds(obj);
            bounds.center = obj.transform.position;

            if (!IsInsideSpawnArea(bounds) || IsOverlapping(bounds))
            {
                Destroy(obj);
                continue;
            }

            placedBounds.Add(bounds);
            spawnedItems.Add(obj);
            return;
        }

        Debug.LogWarning("Konnte fixes Objekt nicht platzieren: " + prefab.name);
    }

    // ================= RANDOM ITEMS =================

    void TryPlaceRandom(Vector3 position)
    {
        if (randomPrefabs == null || randomPrefabs.Count == 0) return;

        GameObject prefab = randomPrefabs[Random.Range(0, randomPrefabs.Count)];

        Quaternion rot = randomYRotation
            ? Quaternion.Euler(0, Random.Range(0, 360), 0)
            : Quaternion.identity;

        GameObject obj = Instantiate(prefab, position, rot, transform);

        ScaleToFit(obj);

        Bounds bounds = GetObjectBounds(obj);
        bounds.center = obj.transform.position;

        if (!IsInsideSpawnArea(bounds) || IsOverlapping(bounds))
        {
            Destroy(obj);
            return;
        }

        placedBounds.Add(bounds);
        spawnedItems.Add(obj);
    }

    // ================= HELPERS =================

    Vector3 GetRandomPointInBounds()
    {
        Bounds b = spawnArea.bounds;

        return new Vector3(
            Random.Range(b.min.x, b.max.x),
            Random.Range(b.min.y, b.max.y),
            Random.Range(b.min.z, b.max.z)
        );
    }

    bool IsInsideSpawnArea(Bounds b)
    {
        Bounds area = spawnArea.bounds;
        return area.Contains(b.min) && area.Contains(b.max);
    }

    bool IsOverlapping(Bounds newBounds)
    {
        foreach (Bounds b in placedBounds)
        {
            if (b.Intersects(newBounds))
                return true;
        }
        return false;
    }

    void ScaleToFit(GameObject obj)
    {
        Bounds itemBounds = GetObjectBounds(obj);
        Bounds areaBounds = spawnArea.bounds;

        Vector3 itemSize = itemBounds.size;
        Vector3 areaSize = areaBounds.size;

        float scaleX = areaSize.x / itemSize.x;
        float scaleY = areaSize.y / itemSize.y;
        float scaleZ = areaSize.z / itemSize.z;

        float scaleFactor = Mathf.Min(scaleX, scaleY, scaleZ);

        scaleFactor *= fitMultiplier;
        scaleFactor = Mathf.Clamp(scaleFactor, minScale, maxScale);

        obj.transform.localScale *= scaleFactor;
    }

    Bounds GetObjectBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        return bounds;
    }

    public void ClearSpawned()
    {
        foreach (GameObject obj in spawnedItems)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedItems.Clear();
        placedBounds.Clear();
    }
}