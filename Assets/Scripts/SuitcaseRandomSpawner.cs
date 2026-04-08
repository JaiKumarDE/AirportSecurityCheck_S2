using System.Collections.Generic;
using UnityEngine;

public class SuitcaseRandomSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnLayer
    {
        public string name;
        public Transform layerTransform;

        [Header("Grid Size (WORLD UNITS)")]
        [Tooltip("Breite einer Zelle in Welt-Einheiten")]
        public float cellSizeX = 0.18f;

        [Tooltip("Tiefe einer Zelle in Welt-Einheiten")]
        public float cellSizeZ = 0.18f;

        [Header("Grid Count")]
        public int columns = 6;
        public int rows = 4;

        [Header("Spawn Count")]
        public int minItems = 2;
        public int maxItems = 6;

        [Header("Possible Prefabs")]
        public GameObject[] possiblePrefabs;

        [Header("Optional")]
        [Tooltip("Kleiner Offset nach oben, damit Items nicht im Boden stecken")]
        public float yOffset = 0.01f;
    }

    [Header("Layers")]
    [SerializeField] private SpawnLayer[] layers = new SpawnLayer[3];

    [Header("General")]
    [SerializeField] private Transform spawnedItemsParent;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Interior Bounds")]
    [Tooltip("BoxCollider, der den Innenraum des Koffers beschreibt.")]
    [SerializeField] private BoxCollider interiorBoundsCollider;

    [Header("Auto Scale")]
    [SerializeField] private bool autoScalePrefabs = true;
    [SerializeField][Range(0.1f, 1f)] private float fitPercent = 0.9f;
    [SerializeField] private float minScaleMultiplier = 0.05f;
    [SerializeField] private float maxScaleMultiplier = 10f;

    [Header("Fallback Rotation")]
    [SerializeField] private bool rotateFlatIfOutside = true;
    [SerializeField] private float flatXRotation = -90f;
    [SerializeField][Range(0.5f, 1f)] private float extraShrinkIfStillOutside = 0.9f;
    [SerializeField] private int fitAttemptsAfterRotation = 3;

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
        if (layer == null) return;
        if (layer.layerTransform == null) return;
        if (layer.possiblePrefabs == null || layer.possiblePrefabs.Length == 0) return;

        bool[,] occupied = new bool[layer.columns, layer.rows];
        int targetCount = Random.Range(layer.minItems, layer.maxItems + 1);
        int placedCount = 0;
        int safety = 1000;

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
            if (validCells.Count == 0)
                break;

            Vector2Int chosen = validCells[Random.Range(0, validCells.Count)];
            MarkOccupied(occupied, chosen.x, chosen.y, sizeX, sizeZ);

            Vector3 spawnPos = GetCellWorldPosition(layer, chosen.x, chosen.y, sizeX, sizeZ);
            Quaternion worldRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            Transform parent = spawnedItemsParent != null ? spawnedItemsParent : transform;

            GameObject obj = Instantiate(prefab, spawnPos, worldRot);
            obj.name = "SPAWNED_" + prefab.name + "_" + layer.name + "_" + placedCount;
            obj.transform.SetParent(parent, true);

            if (autoScalePrefabs)
            {
                FitObjectIntoReservedCells(obj, layer, sizeX, sizeZ);
            }

            RaiseObjectToRestOnLayer(obj, layer);

            ResolveOutsideBounds(obj, layer, sizeX, sizeZ);

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
                if (occupied[x, z])
                    return false;
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
        Vector3 lossy = layer.layerTransform.lossyScale;

        float safeScaleX = Mathf.Abs(lossy.x);
        float safeScaleZ = Mathf.Abs(lossy.z);

        if (safeScaleX < 0.0001f) safeScaleX = 1f;
        if (safeScaleZ < 0.0001f) safeScaleZ = 1f;

        float localCellSizeX = layer.cellSizeX / safeScaleX;
        float localCellSizeZ = layer.cellSizeZ / safeScaleZ;

        float totalWidthLocal = layer.columns * localCellSizeX;
        float totalDepthLocal = layer.rows * localCellSizeZ;

        float startXLocal = -totalWidthLocal * 0.5f;
        float startZLocal = -totalDepthLocal * 0.5f;

        float posXLocal = startXLocal + (cellX * localCellSizeX) + (sizeX * localCellSizeX * 0.5f);
        float posZLocal = startZLocal + (cellZ * localCellSizeZ) + (sizeZ * localCellSizeZ * 0.5f);

        Vector3 localPoint = new Vector3(posXLocal, layer.yOffset, posZLocal);
        return layer.layerTransform.TransformPoint(localPoint);
    }

    private void FitObjectIntoReservedCells(GameObject obj, SpawnLayer layer, int sizeX, int sizeZ)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0)
            return;

        Bounds bounds = GetCombinedRendererBounds(renderers);

        float currentWidth = bounds.size.x;
        float currentDepth = bounds.size.z;

        if (currentWidth <= 0.0001f || currentDepth <= 0.0001f)
            return;

        float targetWidth = layer.cellSizeX * sizeX * fitPercent;
        float targetDepth = layer.cellSizeZ * sizeZ * fitPercent;

        float widthScaleFactor = targetWidth / currentWidth;
        float depthScaleFactor = targetDepth / currentDepth;

        float uniformScaleFactor = Mathf.Min(widthScaleFactor, depthScaleFactor);
        uniformScaleFactor = Mathf.Clamp(uniformScaleFactor, minScaleMultiplier, maxScaleMultiplier);

        obj.transform.localScale *= uniformScaleFactor;
    }

    private void RaiseObjectToRestOnLayer(GameObject obj, SpawnLayer layer)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0)
            return;

        Bounds combinedBounds = GetCombinedRendererBounds(renderers);

        float bottomY = combinedBounds.min.y;
        float targetY = layer.layerTransform.position.y + layer.yOffset;

        float delta = targetY - bottomY;
        obj.transform.position += new Vector3(0f, delta, 0f);
    }

    private void ResolveOutsideBounds(GameObject obj, SpawnLayer layer, int sizeX, int sizeZ)
    {
        if (interiorBoundsCollider == null)
            return;

        if (IsObjectInsideInterior(obj))
            return;

        if (!rotateFlatIfOutside)
            return;

        float randomY = Random.Range(0f, 360f);
        obj.transform.rotation = Quaternion.Euler(flatXRotation, randomY, 0f);

        if (autoScalePrefabs)
        {
            FitObjectIntoReservedCells(obj, layer, sizeX, sizeZ);
        }

        RaiseObjectToRestOnLayer(obj, layer);

        for (int i = 0; i < fitAttemptsAfterRotation; i++)
        {
            if (IsObjectInsideInterior(obj))
                return;

            obj.transform.localScale *= extraShrinkIfStillOutside;
            RaiseObjectToRestOnLayer(obj, layer);
        }
    }

    private bool IsObjectInsideInterior(GameObject obj)
    {
        if (interiorBoundsCollider == null)
            return true;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0)
            return true;

        Bounds objectBounds = GetCombinedRendererBounds(renderers);
        Bounds interior = interiorBoundsCollider.bounds;

        return
            objectBounds.min.x >= interior.min.x &&
            objectBounds.max.x <= interior.max.x &&
            objectBounds.min.y >= interior.min.y &&
            objectBounds.max.y <= interior.max.y &&
            objectBounds.min.z >= interior.min.z &&
            objectBounds.max.z <= interior.max.z;
    }

    private Bounds GetCombinedRendererBounds(Renderer[] renderers)
    {
        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }
        return combinedBounds;
    }

    private void OnDrawGizmosSelected()
    {
        if (layers != null)
        {
            foreach (var layer in layers)
            {
                if (layer == null || layer.layerTransform == null) continue;

                Vector3 origin = layer.layerTransform.TransformPoint(new Vector3(0f, layer.yOffset, 0f));
                Vector3 right = layer.layerTransform.right;
                Vector3 forward = layer.layerTransform.forward;

                Vector3 bottomLeft =
                    origin
                    - right * (layer.columns * layer.cellSizeX * 0.5f)
                    - forward * (layer.rows * layer.cellSizeZ * 0.5f);

                Gizmos.color = Color.cyan;

                for (int x = 0; x <= layer.columns; x++)
                {
                    Vector3 from = bottomLeft + right * (x * layer.cellSizeX);
                    Vector3 to = from + forward * (layer.rows * layer.cellSizeZ);
                    Gizmos.DrawLine(from, to);
                }

                for (int z = 0; z <= layer.rows; z++)
                {
                    Vector3 from = bottomLeft + forward * (z * layer.cellSizeZ);
                    Vector3 to = from + right * (layer.columns * layer.cellSizeX);
                    Gizmos.DrawLine(from, to);
                }

                Gizmos.color = Color.red;

                for (int x = 0; x < layer.columns; x++)
                {
                    for (int z = 0; z < layer.rows; z++)
                    {
                        Vector3 p = GetCellWorldPosition(layer, x, z, 1, 1);
                        Gizmos.DrawSphere(p, 0.015f);
                    }
                }
            }
        }

        if (interiorBoundsCollider != null)
        {
            Gizmos.color = Color.yellow;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = interiorBoundsCollider.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(interiorBoundsCollider.center, interiorBoundsCollider.size);
            Gizmos.matrix = oldMatrix;
        }
    }
}