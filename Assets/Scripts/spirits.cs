using UnityEngine;
using System.Collections.Generic;

public class SubroomSpiritDiscovery : MonoBehaviour
{
    [Header("Spirit Settings")]
    public List<GameObject> ghostParents;
    public ParticleSystem spiritParticles;

    [Header("Visibility Control")]
    [Range(0, 1)] public float idleVisibility = 0.0f;
    [Range(0, 1)] public float discoveredVisibility = 1.0f;
    public float fadeSpeed = 1.0f; // Slower fade feels more "magical"

    private List<Material> ghostMaterials = new List<Material>();
    private bool hasBeenDiscovered = false; // Changed from 'isInside' to a permanent flag
    private float currentVisibility = 0f;

    void Start()
    {
        currentVisibility = idleVisibility;

        foreach (GameObject parent in ghostParents)
        {
            if (parent == null) continue;

            Renderer[] childrenRenderers = parent.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in childrenRenderers)
            {
                // Create unique instance so we don't affect other room ghosts
                rend.material = new Material(rend.material);
                rend.material.SetFloat("_Ghost_Visibility", currentVisibility);
                ghostMaterials.Add(rend.material);
            }
        }
    }

    void Update()
    {
        // Only transition if we haven't reached full visibility yet
        if (currentVisibility < discoveredVisibility && hasBeenDiscovered)
        {
            currentVisibility = Mathf.MoveTowards(currentVisibility, discoveredVisibility, fadeSpeed * Time.deltaTime);

            foreach (Material mat in ghostMaterials)
            {
                if (mat != null)
                {
                    mat.SetFloat("_Ghost_Visibility", currentVisibility);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            // Lock the state to 'true' forever
            if (!hasBeenDiscovered)
            {
                hasBeenDiscovered = true;

                if (spiritParticles != null)
                {
                    spiritParticles.Stop(); // Kill the mist permanently
                }
            }
        }
    }

    // OnTriggerExit is removed because we want them to stay visible!
}