using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    private static DraggableItem currentDragged;

    private Camera inspectCam;

    private bool isDragging;
    private float dragDistance;
    private Vector3 dragOffset;

    private void Start()
    {
        inspectCam = GameObject.FindGameObjectWithTag("Inspect")?.GetComponent<Camera>();

        Debug.Log("Inspect Kamera: " + inspectCam);

        // Falls kein Collider vorhanden ist
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    private void Update()
    {
        if (inspectCam == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }

        if (isDragging)
        {
            DragObject();
        }
    }

    private void TryStartDrag()
    {
        if (currentDragged != null)
            return;

        Debug.Log("Mausklick erkannt");

        Ray ray = inspectCam.ScreenPointToRay(Input.mousePosition);

        // Nur Objekte auf dem Layer "Ziehen" treffen
        int ziehenLayer = LayerMask.GetMask("Ziehen");

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ziehenLayer))
        {
            Debug.Log("Getroffen: " + hit.transform.name);

            if (hit.transform == transform)
            {
                currentDragged = this;
                isDragging = true;

                dragDistance = Vector3.Distance(
                    inspectCam.transform.position,
                    transform.position
                );

                Vector3 mouseWorldPos = ray.GetPoint(dragDistance);
                dragOffset = transform.position - mouseWorldPos;
            }
        }
    }

    private void StopDrag()
    {
        if (currentDragged == this)
        {
            currentDragged = null;
            isDragging = false;
        }
    }

    private void DragObject()
    {
        Ray ray = inspectCam.ScreenPointToRay(Input.mousePosition);

        Vector3 mouseWorldPos = ray.GetPoint(dragDistance);

        transform.position = mouseWorldPos + dragOffset;
    }
}