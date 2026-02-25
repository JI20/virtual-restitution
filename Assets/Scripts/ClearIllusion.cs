using UnityEngine;

public class HeadlineCleaner : MonoBehaviour
{
    [Tooltip("Sollen die Bilder sofort weg sein oder sanft ausfaden?")]
    public bool fadeOutSoftly = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Finde alle aktiven Headlines in der Szene
            HeadlineBehavior[] activeHeadlines = Object.FindObjectsByType<HeadlineBehavior>(FindObjectsSortMode.None);

            foreach (HeadlineBehavior hb in activeHeadlines)
            {
                if (fadeOutSoftly)
                {
                    hb.StartFadeOut(1.5f); // 1.5 Sekunden Ausblendzeit
                }
                else
                {
                    Destroy(hb.gameObject);
                }
            }
            
            // Optional: Den Cleaner selbst zerstören, damit er nur einmal feuert
            Destroy(gameObject);
        }
    }
}