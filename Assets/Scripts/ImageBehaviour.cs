using UnityEngine;

public class HeadlineBehavior : MonoBehaviour
{
    public enum State { FlyIn, Idle, Aggressive, FadeOut }
    private State currentState = State.Idle;

    [Header("Fly-In (Slow-Fast-Slow)")]
    public float flyInDuration = 2.0f;
    public float sideOffset = 15f;
    public bool comingFromRight = true;
    
    private Vector3 targetAnchorPos;
    private Vector3 flyInStartPos;
    private float flyInTimer = 0f;

    [Header("Persönlicher Abstand & Höhe (Dynamisch)")]
    public float minStopDistance = 1.5f;
    public float maxStopDistance = 5.0f;
    public float minHeightOffset = -0.5f;
    public float maxHeightOffset = 2.5f;
    [Tooltip("Wie schnell sich Abstand und Höhe verändern.")]
    public float oscillationSpeed = 0.5f;

    private float individualStopDistance;
    private float individualHeightOffset;
    private float heightPhaseOffset;
    private float distPhaseOffset;

    [Header("Orbit-Einstellungen")]
    public float minOrbitSpeed = 10f;
    public float maxOrbitSpeed = 30f;
    private float individualOrbitSpeed;
    
    [Header("Verfolgung & Sicht")]
    public float followSpeed = 2f; 
    [Range(0, 1)] public float viewFreedomFactor = 0.6f; 

    [Header("Zustand")]
    public bool isAggressive = false;

    private Renderer meshRenderer;
    private Color targetColor;
    private Transform playerTarget;
    private float orbitDirection;
    private float fadeOutDuration = 1.0f;
    private float fadeOutTimer = 0f;

    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            // Speichert die Ziel-Farbe des Materials (inkl. Textur-Farbe)
            targetColor = meshRenderer.material.color;
            // Startet komplett unsichtbar (Alpha 0)
            Color hideColor = targetColor;
            hideColor.a = 0f;
            meshRenderer.material.color = hideColor;
        }
    }

    void Start()
    {
        // Initialisierung individueller Werte für den Schwarm-Effekt
        individualOrbitSpeed = Random.Range(minOrbitSpeed, maxOrbitSpeed);
        heightPhaseOffset = Random.Range(0f, Mathf.PI * 2);
        distPhaseOffset = Random.Range(0f, Mathf.PI * 2);
        
        targetAnchorPos = transform.position;
        float direction = comingFromRight ? 1f : -1f;
        flyInStartPos = targetAnchorPos + (Vector3.right * sideOffset * direction);
        
        // Bild an Startposition setzen
        transform.position = flyInStartPos;
        
        orbitDirection = Random.value > 0.5f ? 1f : -1f;
        currentState = State.FlyIn;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.FlyIn: HandleFlyIn(); break;
            case State.Idle: HandleIdle(); break;
            case State.Aggressive: HandleAggressive(); break;
            case State.FadeOut: HandleFadeOut(); break;
        }
    }

    // Startet den Auflösungs-Prozess von außen (z.B. via Trigger)
    public void StartFadeOut(float duration)
    {
        fadeOutDuration = duration;
        fadeOutTimer = duration;
        currentState = State.FadeOut;
    }

    private void HandleFlyIn()
    {
        flyInTimer += Time.deltaTime;
        float t = Mathf.Clamp01(flyInTimer / flyInDuration);
        float smoothT = Mathf.SmoothStep(0f, 1f, t);
        
        transform.position = Vector3.Lerp(flyInStartPos, targetAnchorPos, smoothT);

        if (meshRenderer != null)
        {
            Color c = targetColor;
            c.a = t; 
            meshRenderer.material.color = c;
        }

        if (t >= 1f) currentState = isAggressive ? State.Aggressive : State.Idle;
    }

    private void HandleIdle()
    {
        float sway = Mathf.Sin(Time.time + heightPhaseOffset) * 0.05f;
        transform.position = targetAnchorPos + new Vector3(0, sway, 0);
        // Sanfte Rotation im Ruhestand
        transform.Rotate(Vector3.up * 5f * Time.deltaTime);
    }

    private void HandleAggressive()
    {
        if (playerTarget == null) { FindPlayer(); return; }

        // 1. DYNAMIK: Berechne aktuelle Höhe und Abstand (Sinus-Welle)
        float heightT = (Mathf.Sin(Time.time * oscillationSpeed + heightPhaseOffset) + 1f) / 2f;
        individualHeightOffset = Mathf.Lerp(minHeightOffset, maxHeightOffset, heightT);

        float distT = (Mathf.Sin(Time.time * (oscillationSpeed * 0.8f) + distPhaseOffset) + 1f) / 2f;
        individualStopDistance = Mathf.Lerp(minStopDistance, maxStopDistance, distT);

        // 2. POSITIONIERUNG: Zielpunkt relativ zum Spieler
        Vector3 playerPosWithOffset = playerTarget.position + (Vector3.up * individualHeightOffset);
        Vector3 toPlayer = playerPosWithOffset - transform.position;
        float currentDistance = toPlayer.magnitude;

        // 3. BLICKFELD-CHECK: Orbit beschleunigen, wenn im Weg
        float dotProduct = Vector3.Dot(playerTarget.forward, -toPlayer.normalized);
        float activeOrbitSpeed = (dotProduct > viewFreedomFactor) ? individualOrbitSpeed * 3f : individualOrbitSpeed;

        // 4. ORBIT-BEWEGUNG: Kreisen um den Spieler
        transform.RotateAround(playerTarget.position, Vector3.up, activeOrbitSpeed * orbitDirection * Time.deltaTime);

        // 5. ABSTAND HALTEN
        if (currentDistance > individualStopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerPosWithOffset, followSpeed * Time.deltaTime);
        }
        else if (currentDistance < individualStopDistance - 0.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerPosWithOffset, -followSpeed * 0.5f * Time.deltaTime);
        }

        // 6. LESBARKEIT-FIX: Immer zum Spieler schauen (um 180° gedreht für Quads)
        Vector3 dirToPlayer = playerTarget.position - transform.position;
        if (dirToPlayer != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dirToPlayer);
            Quaternion targetRot = lookRot * Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    private void HandleFadeOut()
    {
        fadeOutTimer -= Time.deltaTime;
        if (meshRenderer != null)
        {
            Color c = meshRenderer.material.color;
            // Sanftes Ausfaden über die Zeit
            c.a = Mathf.Clamp01(fadeOutTimer / fadeOutDuration);
            meshRenderer.material.color = c;
        }

        if (fadeOutTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }
}