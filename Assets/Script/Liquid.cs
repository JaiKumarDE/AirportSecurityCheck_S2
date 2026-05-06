using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Liquid : MonoBehaviour
{
    public enum UpdateMode { Normal, UnscaledTime }
    public UpdateMode updateMode;

    [SerializeField] float MaxWobble = 0.03f;
    [SerializeField] float WobbleSpeedMove = 1f;
    [SerializeField] float fillAmount = 0.5f;
    [SerializeField] float Recovery = 1f;
    [SerializeField] float Thickness = 1f;
    [Range(0, 1)]
    public float CompensateShapeAmount;
    [SerializeField] Mesh mesh;
    [SerializeField] Renderer rend;

    Vector3 pos;
    Vector3 lastPos;
    Vector3 velocity;
    Quaternion lastRot;
    Vector3 angularVelocity;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float sinewave;
    float time = 0.5f;
    Vector3 comp;

    void Start()
    {
        GetMeshAndRend();
    }

    private void OnValidate()
    {
        GetMeshAndRend();
    }

    void GetMeshAndRend()
    {
        if (mesh == null)
            mesh = GetComponent<MeshFilter>().sharedMesh;
        if (rend == null)
            rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float deltaTime = 0;
        switch (updateMode)
        {
            case UpdateMode.Normal:
                deltaTime = Time.deltaTime;
                break;
            case UpdateMode.UnscaledTime:
                deltaTime = Time.unscaledDeltaTime;
                break;
        }

        time += deltaTime;

        if (deltaTime != 0)
        {
            // Wobble über Zeit verringern
            wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, deltaTime * Recovery);
            wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, deltaTime * Recovery);

            // Sinuswelle berechnen
            pulse = 2 * Mathf.PI * WobbleSpeedMove;
            sinewave = Mathf.Lerp(sinewave, Mathf.Sin(pulse * time),
                deltaTime * Mathf.Clamp(velocity.magnitude + angularVelocity.magnitude, Thickness, 10));

            wobbleAmountX = wobbleAmountToAddX * sinewave;
            wobbleAmountZ = wobbleAmountToAddZ * sinewave;

            // Velocity berechnen
            velocity = (lastPos - transform.position) / deltaTime;
            angularVelocity = GetAngularVelocity(lastRot, transform.rotation);

            // Velocity zur Wobble addieren
            wobbleAmountToAddX += Mathf.Clamp(
                (velocity.x + (velocity.y * 0.2f) + angularVelocity.z + angularVelocity.y) * MaxWobble,
                -MaxWobble, MaxWobble);
            wobbleAmountToAddZ += Mathf.Clamp(
                (velocity.z + (velocity.y * 0.2f) + angularVelocity.x + angularVelocity.y) * MaxWobble,
                -MaxWobble, MaxWobble);
        }

        rend.sharedMaterial.SetFloat("_WobbleX", wobbleAmountX);
        rend.sharedMaterial.SetFloat("_WobbleZ", wobbleAmountZ);

        UpdatePos(deltaTime);

        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    void UpdatePos(float deltaTime)
    {
        // FIX: Alles in Object Space rechnen, nicht World Space.
        // Dadurch bleibt die Füllmenge beim Kippen stabil.
        Vector3 meshCenter = mesh.bounds.center;

        if (CompensateShapeAmount > 0)
        {
            float lowestLocal = GetLowestPointLocal();
            float offsetFromCenter = meshCenter.y - lowestLocal;

            if (deltaTime != 0)
                comp = Vector3.Lerp(comp, new Vector3(0, offsetFromCenter, 0), deltaTime * 10);
            else
                comp = new Vector3(0, offsetFromCenter, 0);

            pos = meshCenter - new Vector3(0, fillAmount - (comp.y * CompensateShapeAmount), 0);
        }
        else
        {
            pos = meshCenter - new Vector3(0, fillAmount, 0);
        }

        rend.sharedMaterial.SetVector("_FillAmount", pos);
    }

    // https://forum.unity.com/threads/manually-calculate-angular-velocity-of-gameobject.289462/#post-4302796
    Vector3 GetAngularVelocity(Quaternion foreLastFrameRotation, Quaternion lastFrameRotation)
    {
        var q = lastFrameRotation * Quaternion.Inverse(foreLastFrameRotation);
        if (Mathf.Abs(q.w) > 1023.5f / 1024.0f)
            return Vector3.zero;

        float gain;
        if (q.w < 0.0f)
        {
            var angle = Mathf.Acos(-q.w);
            gain = -2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
        }
        else
        {
            var angle = Mathf.Acos(q.w);
            gain = 2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
        }

        Vector3 angularVelocity = new Vector3(q.x * gain, q.y * gain, q.z * gain);

        if (float.IsNaN(angularVelocity.z))
            angularVelocity = Vector3.zero;

        return angularVelocity;
    }

    // Niedrigsten Punkt in LOCAL/Object Space (keine Weltkoordinaten!)
    float GetLowestPointLocal()
    {
        float lowestY = float.MaxValue;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y < lowestY)
                lowestY = vertices[i].y;
        }
        return lowestY;
    }
}