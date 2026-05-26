using UnityEngine;

public class ScanAreaFarbe : MonoBehaviour
{
    public string scanAreaTag = "XRayArea";

    [Header("Materialien")]
    public Material normalesMaterial;
    public Material scanMaterial;

    private Renderer rend;
    private MaterialPropertyBlock block;
    private ScanColorManager manager;

    void Start()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();

        manager = FindObjectOfType<ScanColorManager>();

        // Standard Material setzen
        if (normalesMaterial != null)
        {
            rend.material = normalesMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(scanAreaTag)) return;

        // Shader Material aktivieren
        if (scanMaterial != null)
        {
            rend.material = scanMaterial;
        }

        TagFarbe eintrag = manager.GetEintrag(gameObject.tag);
        if (eintrag == null) return;

        Color c = eintrag.farbe;
        c.a = eintrag.alpha;

        block.SetColor("_Color", c);
        rend.SetPropertyBlock(block);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(scanAreaTag)) return;

        // Zurück auf normales Material
        if (normalesMaterial != null)
        {
            rend.material = normalesMaterial;
        }

        block.Clear();
        rend.SetPropertyBlock(block);
    }
}