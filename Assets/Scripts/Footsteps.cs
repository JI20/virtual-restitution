using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [Header("Einstellungen")]
    public AudioSource audioSource;
    public AudioClip[] stepSounds;

    [Header("Geschwindigkeit")]
    [Tooltip("Zeit in Sekunden zwischen Schritten.")]
    [Range(0.2f, 0.8f)] 
    public float stepInterval = 0.35f;

    // NEU: Ein etwas höherer Schwellenwert für VR, um Zittern zu ignorieren
    [Tooltip("Wie schnell muss man sein, damit Schritte ausgelöst werden?")]
    public float movementThreshold = 0.5f; 

    private float stepTimer;
    private Vector3 lastPosition;

    void Start()
    {
        if(audioSource == null) audioSource = GetComponent<AudioSource>();
        lastPosition = transform.position;
    }

    void Update()
    {
        // NEU: Wir ignorieren die Höhe (Y-Achse). 
        // Wir erstellen temporäre Vektoren, die nur X und Z beachten.
        Vector3 currentFlatPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 lastFlatPos = new Vector3(lastPosition.x, 0, lastPosition.z);

        // Geschwindigkeit basierend auf horizontaler Bewegung berechnen
        float speed = Vector3.Distance(currentFlatPos, lastFlatPos) / Time.deltaTime;
        
        lastPosition = transform.position; // Position für nächsten Frame merken

        // NEU: Prüfung gegen den neuen, höheren Threshold (statt fest 0.1f)
        if (speed > movementThreshold) 
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayStep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            // Wenn wir stehen (oder nur zittern), Timer resetten
            stepTimer = 0;
        }
    }

    void PlayStep()
    {
        if (stepSounds == null || stepSounds.Length == 0) return;

        int n = Random.Range(0, stepSounds.Length);
        audioSource.volume = Random.Range(0.9f, 1.0f);
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(stepSounds[n]);
    }
}