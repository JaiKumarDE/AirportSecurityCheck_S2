using UnityEngine;

[System.Serializable]
public class TagFarbe
{
    public string tagName;
    public Color farbe;
    [Range(0f, 1f)] public float alpha = 0.5f; // 👈 NEU
}

public class ScanColorManager : MonoBehaviour
{
    public TagFarbe[] tagFarben;

    public TagFarbe GetEintrag(string tag)
    {
        foreach (TagFarbe eintrag in tagFarben)
        {
            if (eintrag.tagName == tag)
            {
                return eintrag;
            }
        }

        return null;
    }
}