using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public enum RelativeDirection { Forward, Backward }

    [Header("Belt Settings")]
    [SerializeField] private float velocity = 30f;
    [SerializeField] private float accelerationStep = 20f;
    [SerializeField] private float minVelocity = 0f;
    [SerializeField] private float maxVelocity = 100f;
    [SerializeField] private RelativeDirection direction = RelativeDirection.Forward;

    [Header("Visual Settings")]
    [SerializeField] private Renderer beltRenderer;

    private Vector2 textureOffset;
    private Rigidbody rb;
    private bool isMoving = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        if (beltRenderer != null)
            beltRenderer.material = Instantiate(beltRenderer.material);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            isMoving = !isMoving;

        if (Input.GetKey(KeyCode.M))
        {
            velocity += accelerationStep * Time.deltaTime;
            velocity = Mathf.Clamp(velocity, minVelocity, maxVelocity);
        }

        if (Input.GetKey(KeyCode.N))
        {
            velocity -= accelerationStep * Time.deltaTime;
            velocity = Mathf.Clamp(velocity, minVelocity, maxVelocity);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            direction = direction == RelativeDirection.Forward
                ? RelativeDirection.Backward
                : RelativeDirection.Forward;
        }

        if (isMoving)
            ScrollTexture();
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;

        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2f);

        foreach (Collider col in colliders)
        {
            Rigidbody colRb = col.attachedRigidbody;
            if (colRb != null && !colRb.isKinematic)
            {
                // Bewege das Objekt nur, wenn das Band aktiv ist
                Vector3 move = -GetDirection() * (velocity / 4f) * Time.fixedDeltaTime;
                colRb.MovePosition(colRb.position + move);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRb = other.attachedRigidbody;
        if (otherRb != null && !otherRb.isKinematic)
        {
            // Geschwindigkeit des neuen Bands übernehmen
            Vector3 beltVelocity = -GetDirection() * (velocity / 4f);
            otherRb.linearVelocity = beltVelocity;
        }
    }

    private Vector3 GetDirection()
    {
        return direction == RelativeDirection.Forward ? transform.forward : -transform.forward;
    }

    private void ScrollTexture()
    {
        if (beltRenderer == null) return;

        float beltLength = beltRenderer.bounds.size.z;
        if (beltLength <= 0.001f) return;

        float uvSpeed = velocity / beltLength;
        float directionMultiplier = direction == RelativeDirection.Forward ? 1f : -1f;

        textureOffset.y -= uvSpeed * Time.deltaTime * directionMultiplier;
        textureOffset.y %= 1f;

        beltRenderer.material.mainTextureOffset = textureOffset;
    }
}