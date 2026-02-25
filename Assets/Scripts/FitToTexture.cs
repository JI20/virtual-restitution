using UnityEngine;

public class FitToTexture : MonoBehaviour
{
    [Tooltip("Die gewünschte Höhe des Quads in Unity-Einheiten.")]
    public float baseSize = 1.0f;

    void Start()
    {
        ApplyFit();
    }

    void ApplyFit()
    {
        Renderer rend = GetComponent<Renderer>();
        
        // Wir nutzen sharedMaterial, um zu verhindern, dass Unity 
        // während der Größenberechnung Material-Instanzen im Speicher leakt
        if (rend != null && rend.sharedMaterial != null && rend.sharedMaterial.mainTexture != null)
        {
            float width = rend.sharedMaterial.mainTexture.width;
            float height = rend.sharedMaterial.mainTexture.height;

            // Seitenverhältnis berechnen (Breite durch Höhe)
            float ratio = width / height;

            // Die neue Skalierung:
            // X = Höhe * Verhältnis (ergibt die korrekte Breite)
            // Y = Höhe
            transform.localScale = new Vector3(baseSize * ratio, baseSize, 1f);
        }

        // Wir zerstören nur die Komponente, damit das Objekt bleibt, 
        // aber das Skript nicht mehr im Update-Zyklus liegt.
        Destroy(this);
    }
}