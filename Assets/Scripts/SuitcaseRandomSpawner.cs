using System.Collections.Generic;
using UnityEngine;

public class SuitcaseRandomSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnLayer
    {
        public string name;
        public Transform layerTransform;

        [Header("Grid")]
        public int columns = 6;
        public int rows = 4;
        public float cellSizeX = 0.18f;
        public float cellSizeZ = 0.18f;

        [Header("Spawn Count")]
        public int minItems = 2;
        public int maxItems = 6;

        [Header("Possible Prefabs")]
        public GameObject[] possiblePrefabs;
    }

    [SerializeField] private SpawnLayer[] layers = new SpawnLayer[3];
    [SerializeField] private Transform spawnedItemsParent;
    [SerializeField] private bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnAllLayers();
        }
    }

    [ContextMenu("Spawn All Layers")]
    public void SpawnAllLayers()
    {
        ClearSpawned();

        foreach (var layer in layers)
        {
            SpawnLayerItems(layer);
        }
    }

    [ContextMenu("Clear Spawned")]
    public void ClearSpawned()
    {
        Transform parent = spawnedItemsParent != null ? spawnedItemsParent : transform;

        List<GameObject> toDelete = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name.StartsWith("SPAWNED_"))
            {
                toDelete.Add(child.gameObject);
            }
        }

        for (int i = 0; i < toDelete.Count; i++)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(toDelete[i]);
            else
                Destroy(toDelete[i]);
#else
            Destroy(toDelete[i]);
#endif
        }
    }

    private void SpawnLayerItems(SpawnLayer layer)
    {
        if (layer.layerTransform == null) return;
        if (layer.possiblePrefabs == null || layer.possiblePrefabs.Length == 0) return;

        bool[,] occupied = new bool[layer.columns, layer.rows];
        int targetCount = Random.Range(layer.minItems, layer.maxItems + 1);
        int placedCount = 0;
        int safety = 500;

        while (placedCount < targetCount && safety-- > 0)
        {
            GameObject prefab = layer.possiblePrefabs[Random.Range(0, layer.possiblePrefabs.Length)];
            if (prefab == null) continue;

            int sizeX = 1;
            int sizeZ = 1;

            GridItemSize size = prefab.GetComponent<GridItemSize>();
            if (size != null)
            {
                sizeX = Mathf.Max(1, size.sizeX);
                sizeZ = Mathf.Max(1, size.sizeZ);
            }

            List<Vector2Int> validCells = GetValidCells(layer, occupied, sizeX, sizeZ);
            if (validCells.Count == 0) break;

            Vector2Int chosen = validCells[Random.Range(0, validCells.Count)];

            MarkOccupied(occupied, chosen.x, chosen.y, sizeX, sizeZ);

            Vector3 spawnPos = GetCellWorldPosition(layer, chosen.x, chosen.y, sizeX, sizeZ);
            Quaternion rot = Quaternion.Euler(0f, Random.Range(0, 360f), 0f);

            Transform parent = spawnedItemsParent != null ? spawnedItemsParent : transform;
            GameObject obj = Instantiate(prefab, spawnPos, rot, parent);
            obj.name = "SPAWNED_" + prefab.name + "_" + layer.name + "_" + placedCount;

            placedCount++;
        }
    }

    private List<Vector2Int> GetValidCells(SpawnLayer layer, bool[,] occupied, int sizeX, int sizeZ)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for (int x = 0; x <= layer.columns - sizeX; x++)
        {
            for (int z = 0; z <= layer.rows - sizeZ; z++)
            {
                if (CanPlace(occupied, x, z, sizeX, sizeZ))
                {
                    result.Add(new Vector2Int(x, z));
                }
            }
        }

        return result;
    }

    private bool CanPlace(bool[,] occupied, int startX, int startZ, int sizeX, int sizeZ)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int z = startZ; z < startZ + sizeZ; z++)
            {
                if (occupied[x, z]) return false;
            }
        }

        return true;
    }

    private void MarkOccupied(bool[,] occupied, int startX, int startZ, int sizeX, int sizeZ)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int z = startZ; z < startZ + sizeZ; z++)
            {
                occupied[x, z] = true;
            }
        }
    }

    private Vector3 GetCellWorldPosition(SpawnLayer layer, int cellX, int cellZ, int sizeX, int sizeZ)
    {
        float totalWidth = layer.columns * layer.cellSizeX;
        float totalDepth = layer.rows * layer.cellSizeZ;

        float startX = -totalWidth * 0.5f;
        float startZ = -totalDepth * 0.5f;

        float posX = startX + (cellX * layer.cellSizeX) + (sizeX * layer.cellSizeX * 0.5f);
        float posZ = startZ + (cellZ * layer.cellSizeZ) + (sizeZ * layer.cellSizeZ * 0.5f);

        return layer.layerTransform.TransformPoint(new Vector3(posX, 0f, posZ));
    }

    private void OnDrawGizmosSelected()
    {
        if (layers == null) return;

        Gizmos.color = Color.cyan;

        foreach (var layer in layers)
        {
            if (layer == null || layer.layerTransform == null) continue;

            float totalWidth = layer.columns * layer.cellSizeX;
            float totalDepth = layer.rows * layer.cellSizeZ;

            Vector3 origin = layer.layerTransform.position;
            Vector3 right = layer.layerTransform.right;
            Vector3 forward = layer.layerTransform.forward;

            Vector3 bottomLeft = origin - right * totalWidth * 0.5f - forward * totalDepth * 0.5f;

            for (int x = 0; x <= layer.columns; x++)
            {
                Vector3 from = bottomLeft + right * (x * layer.cellSizeX);
                Vector3 to = from + forward * totalDepth;
                Gizmos.DrawLine(from, to);
            }

            for (int z = 0; z <= layer.rows; z++)
            {
                Vector3 from = bottomLeft + forward * (z * layer.cellSizeZ);
                Vector3 to = from + right * totalWidth;
                Gizmos.DrawLine(from, to);
            }
        }
    }
}