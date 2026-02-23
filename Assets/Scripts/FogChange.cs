using System.Collections;
using UnityEngine;

public class FogZoneTrigger : MonoBehaviour
{
    [Header("Fog Settings")]
    [Tooltip("The density the fog will permanently change to.")]
    public float targetFogDensity = 0.01f; 
    
    [Tooltip("How many seconds it takes to transition to the new fog.")]
    public float transitionSpeed = 2.0f;

    // This lock prevents the script from ever running a second time
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player AND if we haven't triggered this yet
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true; // Lock the trigger permanently
            StartCoroutine(LerpFog(targetFogDensity));
        }
    }

    private IEnumerator LerpFog(float target)
    {
        // Safety check: ensure fog is actually turned on
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;

        // Grab whatever the exact fog density happens to be right now
        float startValue = RenderSettings.fogDensity;
        float elapsed = 0f;

        // Smoothly transition over time
        while (elapsed < transitionSpeed)
        {
            RenderSettings.fogDensity = Mathf.Lerp(startValue, target, elapsed / transitionSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to the exact final value just to be clean
        RenderSettings.fogDensity = target;
    }
}