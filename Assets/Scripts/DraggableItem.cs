using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    private static DraggableItem currentDragged;

    [SerializeField] private float smoothSpeed = 20f;

    private Camera inspectCam;

    private bool isDragging;
    private float dragDistance;
    private Vector3 dragOffset;
    private Vector3 targetPosition;
    private Vector3 velocity;

    private void Awake()
    {
        inspectCam = GameObject.FindGameObjectWithTag("Inspect")?.GetComponent<Camera>();

        // Sicherheit: Collider automatisch hinzufügen
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (inspectCam == null)
        {
            inspectCam = GameObject.FindGameObjectWithTag("Inspect")?.GetComponent<Camera>();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }

        if (isDragging)
        {
            UpdateDrag();
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            0.05f
        );
    }

    private void StartDrag()
    {
        if (currentDragged != null)
            return;

        Ray ray = inspectCam.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            1000f,
            ~0,
            QueryTriggerInteraction.Collide
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == transform ||
                hit.transform.IsChildOf(transform) ||
                transform.IsChildOf(hit.transform))
            {
                currentDragged = this;
                isDragging = true;

                dragDistance = Vector3.Distance(
                    inspectCam.transform.position,
                    transform.position
                );

                Vector3 mouseWorldPos = ray.GetPoint(dragDistance);
                dragOffset = transform.position - mouseWorldPos;

                return;
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

    private void UpdateDrag()
    {
        Ray ray = inspectCam.ScreenPointToRay(Input.mousePosition);

        Vector3 mouseWorldPos = ray.GetPoint(dragDistance);

        targetPosition = mouseWorldPos + dragOffset;
    }
}