using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Footsteps : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] defaultSounds;

    [Header("Timing")]
    [Tooltip("Seconds between footstep sounds.")]
    [Range(0.2f, 0.8f)]
    public float stepInterval = 0.35f;
    [Tooltip("Minimum horizontal speed to trigger footsteps.")]
    public float movementThreshold = 0.5f;

    [Header("Surface Zones")]
    [Tooltip("Assign trigger colliders here. When the player enters one, its sounds override the default.")]
    public FootstepZone[] zones;

    [System.Serializable]
    public class FootstepZone
    {
        public string zoneName;
        public Collider trigger;
        public AudioClip[] sounds;
    }

    // --- private state ---
    private float   stepTimer;
    private Vector3 lastPosition;

    // Stack of active zones — supports overlapping zones, most recent wins
    private readonly List<FootstepZone> activeZones = new();

    private AudioClip[] CurrentSounds =>
        activeZones.Count > 0 ? activeZones[^1].sounds : defaultSounds;

    // -------------------------------------------------------------------------

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        lastPosition = transform.position;
        stepTimer    = stepInterval * 0.5f; // small initial delay on scene start
    }

    void Update()
    {
        Vector3 currentFlat = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 lastFlat    = new Vector3(lastPosition.x,       0f, lastPosition.z);

        float speed    = Vector3.Distance(currentFlat, lastFlat) / Time.deltaTime;
        lastPosition   = transform.position;

        // Timer always counts down — never frozen, never zeroed
        stepTimer -= Time.deltaTime;

        if (speed > movementThreshold)
        {
            if (stepTimer <= 0f)
            {
                PlayStep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            // When stopping, ensure a minimum cooldown before the next step fires.
            // This prevents the instant-sound burst when rapidly changing direction.
            if (stepTimer < stepInterval * 0.4f)
                stepTimer = stepInterval * 0.4f;
        }
    }

    // CharacterController triggers OnTriggerEnter/Exit on the same GameObject
    void OnTriggerEnter(Collider other)
    {
        foreach (var zone in zones)
        {
            if (zone.trigger == other && zone.sounds != null && zone.sounds.Length > 0)
            {
                activeZones.Add(zone);
                return;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        for (int i = activeZones.Count - 1; i >= 0; i--)
        {
            if (activeZones[i].trigger == other)
            {
                activeZones.RemoveAt(i);
                return;
            }
        }
    }

    void PlayStep()
    {
        AudioClip[] sounds = CurrentSounds;
        if (sounds == null || sounds.Length == 0) return;

        AudioClip clip = sounds[Random.Range(0, sounds.Length)];
        audioSource.volume = Random.Range(0.9f,  1.0f);
        audioSource.pitch  = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(clip);
    }
}
