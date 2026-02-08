using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Wichtig

public class BridgeManagerVR : MonoBehaviour
{
    // HINWEIS: Das Feld "locomotionSystemObj" brauchen wir jetzt gar nicht mehr zwingend,
    // weil wir selbst suchen. Du kannst es ignorieren.
    public GameObject locomotionSystemObj; 
    
    public Camera vrCamera;

    [Header("Die Strecke")]
    public Transform bridgeStart;
    public Transform bridgeEnd;

    [Header("Einstellungen")]
    public float normalSpeed = 2.5f;
    public float nightmareSpeed = 0.0f;
    public float startFOV = 60f;
    public float endFOV = 110f;

    private ContinuousMoveProviderBase moveEngine;
    private float totalDistance;

    void Start()
    {
        // 1. Gesamtlänge berechnen
        totalDistance = Vector3.Distance(bridgeStart.position, bridgeEnd.position);

        // --- DER FIX: WIR SUCHEN IN DER GANZEN SZENE ---
        
        // FindFirstObjectByType sucht überall in der Szene nach diesem Typ.
        // Das ist etwas langsamer, aber im Start() ist das egal.
        moveEngine = FindFirstObjectByType<ContinuousMoveProviderBase>();

        if (moveEngine == null)
        {
            Debug.LogError("KRITISCHER FEHLER: In der ganzen Szene gibt es keinen 'Continuous Move Provider'!");
            Debug.LogError("Kannst du dich überhaupt mit dem Joystick bewegen? Wenn nein, fehlt dir die Komponente.");
        }
        else
        {
            Debug.Log("Gefunden! Der Motor ist auf dem Objekt: " + moveEngine.gameObject.name);
        }
    }

    void Update()
    {
        if (vrCamera == null) return;

        Vector3 bridgeDir = (bridgeEnd.position - bridgeStart.position).normalized;
        Vector3 playerToStart = vrCamera.transform.position - bridgeStart.position;
        
        float currentDist = Vector3.Dot(playerToStart, bridgeDir);
        float progress = Mathf.Clamp01(currentDist / totalDistance);

        // Geschwindigkeit steuern (nur wenn Motor gefunden wurde)
        if (moveEngine != null)
        {
            moveEngine.moveSpeed = Mathf.Lerp(normalSpeed, nightmareSpeed, progress * progress);
        }

        vrCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, progress);
    }
}