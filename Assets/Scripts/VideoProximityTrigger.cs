using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoProximityTrigger : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Renderer screenRenderer; 
    public float fadeDuration = 2.0f; // How long the fade takes
    
    private Coroutine fadeRoutine;
    private Material screenMat;
    private static readonly int _BaseColor = Shader.PropertyToID("_BaseColor"); // For URP
    private static readonly int _MainColor = Shader.PropertyToID("_Color");     // For Built-in

    void Start()
    {
        screenMat = screenRenderer.material;
        
        // Initial State: Hidden and Silent
        SetAlpha(0);
        videoPlayer.SetDirectAudioVolume(0, 0); 
        screenRenderer.enabled = false; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(Fade(1f)); // Fade In
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(Fade(0f)); // Fade Out
        }
    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = screenMat.color.a;
        float timer = 0;

        if (targetAlpha > 0)
        {
            videoPlayer.Prepare();

            // Wait for preparation AND for the first frame to be ready
            while (!videoPlayer.isPrepared) yield return null;
        
            // This ensures the turquoise buffer is overwritten by frame 0
            videoPlayer.Play(); 
        
            // Give the GPU one or two frames to actually swap the textures
            yield return new WaitForEndOfFrame(); 
            yield return new WaitForEndOfFrame();

            screenRenderer.enabled = true;
        }

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            
            // Use Lerp for a smooth mathematical transition
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            
            SetAlpha(currentAlpha);
            videoPlayer.SetDirectAudioVolume(0, currentAlpha);
            yield return null;
        }

        // Final check: if we faded OUT, disable the mesh and pause
        if (targetAlpha <= 0)
        {
            videoPlayer.Pause();
            screenRenderer.enabled = false;
        }
    }

    void SetAlpha(float alpha)
    {
        Color c = screenMat.color;
        c.a = alpha;
        screenMat.color = c;
        
        // Ensure the shader actually updates the alpha property
        if (screenMat.HasProperty("_BaseColor")) screenMat.SetColor("_BaseColor", c);
        else if (screenMat.HasProperty("_Color")) screenMat.SetColor("_Color", c);
    }
}