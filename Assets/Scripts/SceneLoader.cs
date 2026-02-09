using System.Collections.Generic; // Wichtig für Listen
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Hier kannst du im Inspector die Namen eintragen
    // Tooltip zeigt einen Hilfstext, wenn du mit der Maus drüberfährst
    [Tooltip("Trage hier die exakten Namen der Szenen ein.")]
    public List<string> szenenNamen;

    void Start()
    {
        foreach (string szene in szenenNamen)
        {
            // Prüfung: Ist der Name leer? Falls ja, überspringen.
            if (!string.IsNullOrEmpty(szene))
            {
                // Lädt die Szene im Hintergrund additiv dazu
                SceneManager.LoadSceneAsync(szene, LoadSceneMode.Additive);
            }
        }
    }
}