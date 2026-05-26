using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    private Camera cam;
    private bool dragging;

    private float distance;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        distance = Vector3.Distance(
            transform.position,
            cam.transform.position
        );

        dragging = true;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnMouseUp()
    {
        dragging = false;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    private void Update()
    {
        if (!dragging)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Vector3 point = ray.GetPoint(distance);

        transform.position = point;
    }
}