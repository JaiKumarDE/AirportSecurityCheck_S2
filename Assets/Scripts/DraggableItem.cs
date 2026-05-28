using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    private static DraggableItem currentDragged;

    private Camera cam;

    private bool dragging;

    private float distanceToCamera;

    private Vector3 offset;

    private Vector3 targetPosition;

    [SerializeField]
    private float smoothSpeed = 15f;

    // =====================================================

    private void Start()
    {
        targetPosition = transform.position;
    }

    // =====================================================

    private void Update()
    {
        // IMMER aktive Kamera holen
        cam = GetActiveCamera();

        if (cam == null)
            return;

        // =========================================
        // MAUS KLICK
        // =========================================

        if (Input.GetMouseButtonDown(0))
        {
            TryStartDragging();
        }

        // =========================================
        // MAUS LOS
        // =========================================

        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }

        // =========================================
        // DRAGGING
        // =========================================

        if (dragging)
        {
            Ray ray =
                cam.ScreenPointToRay(
                    Input.mousePosition
                );

            Vector3 mouseWorldPos =
                ray.GetPoint(distanceToCamera);

            targetPosition =
                mouseWorldPos + offset;
        }

        // Smooth movement
        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * smoothSpeed
            );
    }

    // =====================================================

    private void TryStartDragging()
    {
        // Schon ein anderes Objekt aktiv
        if (
            currentDragged != null
            && currentDragged != this
        )
        {
            return;
        }

        Ray ray =
            cam.ScreenPointToRay(
                Input.mousePosition
            );

        if (
            Physics.Raycast(
                ray,
                out RaycastHit hit,
                1000f
            )
        )
        {
            // WICHTIG:
            // Parent check
            if (
                hit.transform == transform
                || hit.transform.IsChildOf(transform)
            )
            {
                currentDragged = this;

                dragging = true;

                distanceToCamera =
                    Vector3.Distance(
                        cam.transform.position,
                        transform.position
                    );

                Vector3 mouseWorldPos =
                    ray.GetPoint(distanceToCamera);

                offset =
                    transform.position
                    - mouseWorldPos;
            }
        }
    }

    // =====================================================

    private void StopDragging()
    {
        if (currentDragged == this)
        {
            dragging = false;

            currentDragged = null;
        }
    }

    // =====================================================

    private Camera GetActiveCamera()
    {
        Camera[] cams = Camera.allCameras;

        foreach (Camera c in cams)
        {
            if (c.enabled && c.gameObject.activeInHierarchy)
            {
                return c;
            }
        }

        return null;
    }
}