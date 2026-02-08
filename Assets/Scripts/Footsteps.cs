using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [Header("Einstellungen")]
    public AudioSource audioSource;
    public AudioClip[] stepSounds;

    [Header("Geschwindigkeit")]
    [Tooltip("Zeit in Sekunden zwischen Schritten. Kleinerer Wert = Schnellere Schritte.")]
    [Range(0.2f, 0.8f)] 
    public float stepInterval = 0.35f; // Standardwert auf 0.35 gesenkt (war 0.5)

    private float stepTimer;
    private Vector3 lastPosition;

    void Start()
    {
        // Falls vergessen, holen wir uns die AudioSource automatisch
        if(audioSource == null) audioSource = GetComponent<AudioSource>();
        
        lastPosition = transform.position;
    }

    void Update()
    {
        // Wir berechnen die Geschwindigkeit rein über die Positionsänderung
        // Das funktioniert mit JEDEM Input-System (Alt oder Neu)
        float speed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;
        
        lastPosition = transform.position;

        // Wenn wir uns schneller als 0.1 Einheiten bewegen
        if (speed > 0.1f) 
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayStep();
                stepTimer = stepInterval; // Timer zurücksetzen
            }
        }
        else
        {
            // Wenn wir stehen, Timer auf 0 setzen, 
            // damit beim Loslaufen der erste Schritt SOFORT kommt
            stepTimer = 0;
        }
    }

    void PlayStep()
    {
        if (stepSounds == null || stepSounds.Length == 0) return;

        // Zufälligen Sound auswählen
        int n = Random.Range(0, stepSounds.Length);

        // Minimale Variation bei Lautstärke und Tonhöhe für mehr Realismus
        audioSource.volume = Random.Range(0.9f, 1.0f);
        audioSource.pitch = Random.Range(0.95f, 1.05f);

        audioSource.PlayOneShot(stepSounds[n]);
    }
}