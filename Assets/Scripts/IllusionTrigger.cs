using UnityEngine;
using System.Collections;

public class IllusionTrigger : MonoBehaviour
{
    public enum Phase { Phase2, Phase3 }
    public Phase triggerPhase;

    [Header("Phase 2 Einstellungen")]
    public GameObject[] hiddenImagesPhase2;
    public float minDelayPhase2 = 0.1f;
    public float maxDelayPhase2 = 0.6f;

    [Header("Phase 3 Einstellungen")]
    [Tooltip("Es werden alle Bilder aus dieser Liste nacheinander gespawnt.")]
    public GameObject[] imagePrefabs; 
    public float spawnRadius = 4f; 
    [Tooltip("Wie breit ist der Fächer vor dem Spieler? (z.B. 90 Grad)")]
    public float spawnAngleRange = 90f;
    [Tooltip("Zeit in Sekunden, in der alle Bilder fertig gespawnt sein müssen.")]
    public float totalSpawnDuration = 2.0f;

    private Transform playerTransform;
    private bool hasTriggered = false;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            if (playerTransform == null) playerTransform = other.transform;

            if (triggerPhase == Phase.Phase2)
            {
                StartCoroutine(ActivatePhase2Routine());
            }
            else if (triggerPhase == Phase.Phase3)
            {
                StartCoroutine(ActivatePhase3());
            }
        }
    }

    IEnumerator ActivatePhase2Routine()
    {
        foreach (GameObject img in hiddenImagesPhase2)
        {
            if (img != null)
            {
                img.SetActive(true);
                yield return new WaitForSeconds(Random.Range(minDelayPhase2, maxDelayPhase2));
            }
        }
    }

    IEnumerator ActivatePhase3()
    {
        if (playerTransform == null || imagePrefabs.Length == 0) yield break;

        // BERECHNUNG DER PAUSE:
        // Damit alle Bilder in genau X Sekunden erscheinen
        float delayBetweenSpawns = totalSpawnDuration / imagePrefabs.Length;

        for (int i = 0; i < imagePrefabs.Length; i++)
        {
            GameObject prefab = imagePrefabs[i];
            if (prefab == null) continue;

            // Position vor dem Spieler berechnen
            float randomAngle = Random.Range(-spawnAngleRange * 0.5f, spawnAngleRange * 0.5f);
            Quaternion spawnRotation = Quaternion.Euler(0, randomAngle, 0);
            Vector3 relativeSpawnPos = spawnRotation * playerTransform.forward * spawnRadius;
            
            float heightOffset = Random.Range(-0.5f, 2.5f); 
            Vector3 targetPos = playerTransform.position + relativeSpawnPos + (Vector3.up * heightOffset);

            GameObject clone = Instantiate(prefab, targetPos, Quaternion.identity);
            
            HeadlineBehavior behavior = clone.GetComponent<HeadlineBehavior>();
            if (behavior != null)
            {
                behavior.isAggressive = true;
                behavior.comingFromRight = (i % 2 == 0);
                behavior.flyInDuration = Random.Range(1.0f, 2.0f); // Etwas schnelleres Einfliegen passend zum schnellen Spawn
            }

            // Hier wird die berechnete Pause genutzt
            yield return new WaitForSeconds(delayBetweenSpawns); 
        }
    }
}