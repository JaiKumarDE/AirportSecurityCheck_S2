using UnityEngine;
using UnityEngine.SceneManagement;

public class ScanAreaFarbe : MonoBehaviour
{
    public string scanAreaTag = "XRayArea";

    private Renderer rend;
    private MaterialPropertyBlock block;
    private ScanColorManager manager;

    void Start()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();

        manager = FindObjectOfType<ScanColorManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(scanAreaTag)) return;

        TagFarbe eintrag = manager.GetEintrag(gameObject.tag);
        if (eintrag == null) return;

        Color c = eintrag.farbe;
        c.a = eintrag.alpha; // 👈 Alpha kommt jetzt vom Manager

        block.SetColor("_Color", c);
        rend.SetPropertyBlock(block);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(scanAreaTag)) return;

        block.Clear();
        rend.SetPropertyBlock(block);
    }
}