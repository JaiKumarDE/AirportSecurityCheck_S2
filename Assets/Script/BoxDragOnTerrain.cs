using UnityEngine;

public class BoxDragOnTerrain : MonoBehaviour
{
    private Camera cam;
    private bool dragging;

    private Vector3 offset;
    private float distanceToCamera;
    private Vector3 targetPosition;

    [SerializeField] private float smoothSpeed = 20f;

    void Start()
    {
        cam = Camera.main;
        targetPosition = transform.position;
    }

    void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
            return;
        }

        // Maus drücken
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        // Maus loslassen
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        // Dragging
        if (dragging)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                targetPosition = hit.point + offset;
            }
        }

        // Smooth movement
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * smoothSpeed
        );
    }

    void TryStartDrag()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            // NUR BoxCollider Objekte erlauben
            BoxCollider box = hit.collider as BoxCollider;
            if (box == null) return;

            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                dragging = true;

                distanceToCamera = Vector3.Distance(cam.transform.position, transform.position);

                Vector3 mouseWorld = ray.GetPoint(distanceToCamera);
                offset = transform.position - mouseWorld;
            }
        }
    }
}