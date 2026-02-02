using UnityEngine;

public class CorrectPitchDjembe : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bassSource;
    public AudioSource slapSource;
    public AudioClip bassClip;
    public AudioClip slapClip;

    [Header("Pitch Tuning (Bass Only)")]
    [Tooltip("The lowest pitch for a dead-center hit.")]
    [Range(0.5f, 1.5f)] public float centerBassPitch = 0.85f;
    [Tooltip("How much the pitch increases as you reach the rim.")]
    public float rimPitchIncrease = 0.3f;

    [Header("Physics Sensitivity")]
    public float velocitySensitivity = 1.5f;
    public float drumRadius = 0.18f;
    public float reTriggerLockout = 0.05f;

    private float _lastHitTime;

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - _lastHitTime < reTriggerLockout) return;

        if (collision.gameObject.CompareTag("DrumStick") || collision.gameObject.CompareTag("PlayerHand"))
        {
            _lastHitTime = Time.time;

            float velocity = collision.relativeVelocity.magnitude;
            float dist = Vector3.Distance(collision.contacts[0].point, drumHeadCenter.position);

            // Normalize distance (0 = Center, 1 = Edge)
            float t = Mathf.Clamp01(dist / drumRadius);

            // Volume logic (Exponential for punch)
            float volume = Mathf.Pow(Mathf.Clamp01(velocity / velocitySensitivity), 0.5f);

            PlayCorrectPitch(t, volume);
        }
    }

    void PlayCorrectPitch(float t, float volume)
    {
        // 1. RIM SOUND (SLAP): Always stays at its natural, crisp pitch
        slapSource.pitch = 1.0f;

        // 2. CENTER SOUND (BASS): Starts low at center (t=0) and rises toward rim (t=1)
        // Formula: Base Pitch + (Distance * Increase)
        bassSource.pitch = centerBassPitch + (t * rimPitchIncrease);

        // 3. BLENDING: 
        // We use a curve so the bass stays dominant until the very edge
        float slapWeight = Mathf.Pow(t, 2);
        float bassWeight = 1f - slapWeight;

        bassSource.PlayOneShot(bassClip, bassWeight * volume);
        slapSource.PlayOneShot(slapClip, slapWeight * volume);
    }

    [Header("Visualizer")]
    public Transform drumHeadCenter;
    private void OnDrawGizmosSelected()
    {
        if (drumHeadCenter == null) return;
        Gizmos.color = Color.green; // Inner 'Sweet' Zone
        Gizmos.DrawWireSphere(drumHeadCenter.position, drumRadius * 0.4f);
        Gizmos.color = Color.blue; // Outer Limit
        Gizmos.DrawWireSphere(drumHeadCenter.position, drumRadius);
    }
}