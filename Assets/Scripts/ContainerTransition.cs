using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContainerTransition : MonoBehaviour
{
    [System.Serializable]
    public struct TimedCue
    {
        [Tooltip("Seconds after the ride starts to play this sound.")]
        public float time;
        public AudioSource sound;
    }

    [Header("Objects to Control")]
    public GameObject leftDoor;
    public GameObject hiddenPackages;
    public Light[] flickerLights;

    [Header("Audio - Door")]
    public AudioSource doorCloseSound;
    public AudioSource doorOpenSound;
    public AudioSource flickerSound;

    [Header("Audio - Transport")]
    [Tooltip("Loops for the entire ride. Great for ocean waves or engine hum.")]
    public AudioSource ambientLoop;

    [Tooltip("Plays at an exact second into the ride. Use for ship horns, crane sounds, etc.")]
    public TimedCue[] timedCues;

    [Tooltip("Random one-shots: metal creaks, thuds, settling sounds.")]
    public AudioSource[] randomSounds;
    public float randomSoundMinInterval = 2f;
    public float randomSoundMaxInterval = 8f;

    [Header("Door Settings")]
    public Vector3 leftDoorOpenRot;
    public Vector3 leftDoorClosedRot;

    [Tooltip("How many seconds the door takes to close. Match this to your close sound!")]
    public float doorCloseDuration = 1.5f;

    [Tooltip("How many seconds the door takes to open. Match this to your open sound!")]
    public float doorOpenDuration = 2.0f;

    [Header("Transport - Shake & Sway")]
    [Tooltip("Root object of the container geometry. This is what gets tilted and vibrated.")]
    public Transform containerRoot;

    [Tooltip("Max degrees of slow side-to-side tilt. Keep this small (1-2) for VR comfort.")]
    public float swayAmount = 1.2f;

    [Tooltip("How fast the sway oscillates. Lower = lazier, like a ship.")]
    public float swaySpeed = 0.5f;

    [Tooltip("Tiny position jitter in Unity units. Keep below 0.01 for VR comfort.")]
    public float vibrationAmount = 0.006f;

    [Tooltip("How fast the vibration changes. Higher = rougher road feel.")]
    public float vibrationSpeed = 25f;

    [Tooltip("Seconds to fade the shake in at the start and out at the end of the ride.")]
    public float shakeFadeDuration = 1.5f;

    [Header("Transport - Shifting Objects")]
    [Tooltip("Rigidbodies (packages etc.) that get nudged during the ride.")]
    public Rigidbody[] shiftingObjects;

    [Tooltip("Impulse force applied to each shifting object when nudged.")]
    public float shiftForce = 0.6f;

    [Tooltip("Seconds between physics nudges on shifting objects.")]
    public float shiftInterval = 5f;

    [Header("Scene Management")]
    public string[] scenesToUnload = { "room1", "room2" };
    public string sceneToLoad = "room3";


    [Header("Sequence Timing")]
    public float totalSequenceTime = 60f;

    private bool sequenceStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !sequenceStarted)
        {
            sequenceStarted = true;
            StartCoroutine(RunShippingSequence());
        }
    }

    private IEnumerator RunShippingSequence()
    {
        // 1. Close the door
        if (doorCloseSound != null) doorCloseSound.Play();
        yield return StartCoroutine(AnimateDoor(leftDoorClosedRot, doorCloseDuration));

        // 2. Flicker lights and reveal packages
        yield return StartCoroutine(FlickerLight());

        // 3. Swap scenes silently in the background
        StartCoroutine(SwapScenes());

        // 4. Run the transport experience — door waits for this to fully complete,
        //    including the ramp-down, before opening
        yield return StartCoroutine(RunTransportExperience());

        // 5. Open the door into the new world
        if (doorOpenSound != null) doorOpenSound.Play();
        yield return StartCoroutine(AnimateDoor(leftDoorOpenRot, doorOpenDuration));
    }

    private IEnumerator RunTransportExperience()
    {
        Vector3 originalPos = containerRoot != null ? containerRoot.localPosition : Vector3.zero;
        Quaternion originalRot = containerRoot != null ? containerRoot.localRotation : Quaternion.identity;

        if (ambientLoop != null)
        {
            ambientLoop.volume = 0f;
            ambientLoop.Play();
        }

        // noiseTime increments continuously across all three phases so Perlin noise
        // and sine waves never jump when transitioning between ramp-up, main, and ramp-down
        float noiseTime = 0f;

        // --- Phase 1: Ramp up (outside totalSequenceTime) ---
        float rampElapsed = 0f;
        while (rampElapsed < shakeFadeDuration)
        {
            float t = rampElapsed / shakeFadeDuration;
            t = t * t * (3f - 2f * t); // smoothstep: eases in
            ApplyShake(containerRoot, originalPos, originalRot, noiseTime, t);
            if (ambientLoop != null) ambientLoop.volume = t;
            noiseTime += Time.deltaTime;
            rampElapsed += Time.deltaTime;
            yield return null;
        }

        // --- Phase 2: Full shake for the ride duration ---
        float elapsed = 0f;
        float nextRandomSound = Random.Range(randomSoundMinInterval, randomSoundMaxInterval);
        float nextShift = shiftInterval;
        bool[] cueFired = new bool[timedCues != null ? timedCues.Length : 0];

        while (elapsed < totalSequenceTime)
        {
            float delta = Time.deltaTime;

            ApplyShake(containerRoot, originalPos, originalRot, noiseTime, 1f);

            // --- Timed cue sounds ---
            for (int i = 0; i < cueFired.Length; i++)
            {
                if (!cueFired[i] && elapsed >= timedCues[i].time)
                {
                    if (timedCues[i].sound != null) timedCues[i].sound.Play();
                    cueFired[i] = true;
                }
            }

            // --- Random one-shot sounds ---
            nextRandomSound -= delta;
            if (nextRandomSound <= 0f && randomSounds != null && randomSounds.Length > 0)
            {
                int idx = Random.Range(0, randomSounds.Length);
                if (randomSounds[idx] != null && !randomSounds[idx].isPlaying)
                    randomSounds[idx].Play();
                nextRandomSound = Random.Range(randomSoundMinInterval, randomSoundMaxInterval);
            }

            // --- Nudge shifting objects ---
            nextShift -= delta;
            if (nextShift <= 0f && shiftingObjects != null)
            {
                foreach (Rigidbody rb in shiftingObjects)
                {
                    if (rb != null)
                        rb.AddForce(Random.insideUnitSphere * shiftForce, ForceMode.Impulse);
                }
                nextShift = shiftInterval;
            }

            noiseTime += delta;
            elapsed += delta;
            yield return null;
        }

        if (ambientLoop != null) ambientLoop.volume = 1f;

        // --- Phase 3: Ramp down (outside totalSequenceTime) ---
        rampElapsed = 0f;
        while (rampElapsed < shakeFadeDuration)
        {
            float t = 1f - (rampElapsed / shakeFadeDuration);
            t = t * t * (3f - 2f * t); // smoothstep: eases out
            ApplyShake(containerRoot, originalPos, originalRot, noiseTime, t);
            if (ambientLoop != null) ambientLoop.volume = t;
            noiseTime += Time.deltaTime;
            rampElapsed += Time.deltaTime;
            yield return null;
        }

        // Snap cleanly to rest and stop audio
        if (containerRoot != null)
        {
            containerRoot.SetLocalPositionAndRotation(originalPos, originalRot);
        }
        if (ambientLoop != null) ambientLoop.Stop();
    }

    private void ApplyShake(Transform root, Vector3 originalPos, Quaternion originalRot, float noiseTime, float intensity)
    {
        if (root == null) return;

        float tiltX = Mathf.Sin(noiseTime * swaySpeed) * swayAmount * intensity;
        float tiltZ = Mathf.Sin(noiseTime * swaySpeed * 0.65f) * swayAmount * 0.6f * intensity;

        float vibX = (Mathf.PerlinNoise(noiseTime * vibrationSpeed, 0f) - 0.5f) * 2f * vibrationAmount * intensity;
        float vibY = (Mathf.PerlinNoise(0f, noiseTime * vibrationSpeed) - 0.5f) * 2f * vibrationAmount * 0.4f * intensity;
        float vibZ = (Mathf.PerlinNoise(noiseTime * vibrationSpeed, noiseTime * vibrationSpeed) - 0.5f) * 2f * vibrationAmount * intensity;

        root.SetLocalPositionAndRotation(
            originalPos + new Vector3(vibX, vibY, vibZ),
            originalRot * Quaternion.Euler(tiltX, 0f, tiltZ));
    }

    private IEnumerator AnimateDoor(Vector3 targetRotation, float duration)
    {
        Quaternion startRot = leftDoor.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(targetRotation);
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);

            leftDoor.transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        leftDoor.transform.localRotation = endRot;
    }

    private IEnumerator SwapScenes()
    {
        foreach (string scene in scenesToUnload)
        {
            if (SceneManager.GetSceneByName(scene).isLoaded)
                yield return SceneManager.UnloadSceneAsync(scene);
        }

        yield return SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        // Make room3 the active scene so its skybox and lighting settings take over
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
    }

    private IEnumerator FlickerLight()
    {
        if (flickerSound != null) flickerSound.Play();

        // Start dark
        SetFlickerLights(false);

        // --- Burst 1: two quick flashes, all lights together ---
        yield return new WaitForSeconds(0.06f);
        SetFlickerLights(true);
        yield return new WaitForSeconds(0.04f);
        SetFlickerLights(false);
        yield return new WaitForSeconds(0.10f);
        SetFlickerLights(true);
        yield return new WaitForSeconds(0.03f);
        SetFlickerLights(false);
        yield return new WaitForSeconds(0.15f);

        // --- Burst 2: lights struggle to come on one at a time ---
        SetFlickerLight(0, true);
        yield return new WaitForSeconds(0.05f);
        SetFlickerLight(1, true);
        yield return new WaitForSeconds(0.03f);
        SetFlickerLights(false);
        yield return new WaitForSeconds(0.09f);
        SetFlickerLight(0, true);
        yield return new WaitForSeconds(0.02f);
        SetFlickerLight(2, true);
        yield return new WaitForSeconds(0.06f);
        SetFlickerLights(false);
        yield return new WaitForSeconds(0.12f);

        // Reveal packages while dark
        if (hiddenPackages != null) hiddenPackages.SetActive(true);

        // --- Final: one last stutter then all settle on ---
        SetFlickerLights(true);
        yield return new WaitForSeconds(0.05f);
        SetFlickerLights(false);
        yield return new WaitForSeconds(0.04f);
        SetFlickerLights(true);
    }

    private void SetFlickerLights(bool on)
    {
        if (flickerLights == null) return;
        foreach (Light l in flickerLights)
            if (l != null) l.enabled = on;
    }

    private void SetFlickerLight(int index, bool on)
    {
        if (flickerLights == null || index >= flickerLights.Length) return;
        if (flickerLights[index] != null) flickerLights[index].enabled = on;
    }
}
