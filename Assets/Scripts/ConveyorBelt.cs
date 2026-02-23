using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public enum RelativeDirection
    {
        Forward,
        Backward
    }

    [Header("Belt Settings")]
    [SerializeField] private float velocity = 30f;
    [SerializeField] private RelativeDirection direction = RelativeDirection.Forward;

    [Header("Visual Settings")]
    [SerializeField] private Renderer beltRenderer;

    private Vector2 textureOffset;
    private Rigidbody rb;
    private bool isMoving = true; // 🔥 Flag ob Conveyor läuft

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (beltRenderer != null)
        {
            beltRenderer.material = Instantiate(beltRenderer.material);
        }
    }

    private void Update()
    {
        // 🔥 Stop / Start mit Leertaste
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isMoving = !isMoving;
        }

        if (isMoving)
        {
            ScrollTexture();
        }
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;

        // 🔥 Bewege alle Rigidbody, die auf dem Conveyor liegen
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2f);
        foreach (Collider col in colliders)
        {
            Rigidbody colRb = col.attachedRigidbody;
            if (colRb != null && !colRb.isKinematic)
            {
                Vector3 move = -GetDirection() * (velocity / 4f) * Time.fixedDeltaTime;
                colRb.MovePosition(colRb.position + move);
            }
        }
    }

    private Vector3 GetDirection()
    {
        switch (direction)
        {
            case RelativeDirection.Forward:
                return transform.forward;
            case RelativeDirection.Backward:
                return -transform.forward;
        }

        return transform.forward;
    }

    private void ScrollTexture()
    {
        if (beltRenderer == null) return;

        float beltLength = beltRenderer.bounds.size.z;
        if (beltLength <= 0.001f) return;

        float uvSpeed = velocity / beltLength;

        // 🔥 Wenn Rückwärts, Textur umdrehen
        float directionMultiplier = (direction == RelativeDirection.Forward) ? 1f : -1f;
        textureOffset.y -= uvSpeed * Time.deltaTime * directionMultiplier;
        textureOffset.y %= 1f;

        beltRenderer.material.mainTextureOffset = textureOffset;
    }
}