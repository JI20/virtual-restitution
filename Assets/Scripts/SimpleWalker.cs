using UnityEngine;
using UnityEngine.InputSystem;

// Das Script kommt direkt auf die MAIN CAMERA
[RequireComponent(typeof(CharacterController))]
public class SimpleWalker : MonoBehaviour
{
    [Header("Bewegung")]
    public float moveSpeed = 5.0f;
    public float gravity = 9.81f;

    [Header("Sicht")]
    public float mouseSensitivity = 0.5f;

    [Header("Tasten")]
    public Key keyForward = Key.W;
    public Key keyBack = Key.S;
    public Key keyLeft = Key.A;
    public Key keyRight = Key.D;

    // Für die Brücke
    public float ForwardInput { get; private set; }

    // Wir speichern beide Winkel
    private float rotationX = 0f; // Hoch/Runter
    private float rotationY = 0f; // Links/Rechts

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Start-Drehung übernehmen, damit die Kamera nicht springt
        Vector3 angles = transform.localEulerAngles;
        rotationX = angles.x;
        rotationY = angles.y;
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        // --- 1. Maus Input (Beide Achsen) ---
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        rotationY += mouseDelta.x * mouseSensitivity; // Links/Rechts addieren
        rotationX -= mouseDelta.y * mouseSensitivity; // Hoch/Runter abziehen
        
        // Begrenzen, damit man keinen Rückwärtssalto macht
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // JETZT KOMMT DER TRICK:
        // Wir wenden BEIDE Drehungen auf dieses EINE Objekt (Kamera) an.
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);


        // --- 2. Bewegung ---
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current[keyForward].isPressed) moveZ += 1f;
        if (Keyboard.current[keyBack].isPressed) moveZ -= 1f;
        if (Keyboard.current[keyLeft].isPressed) moveX -= 1f;
        if (Keyboard.current[keyRight].isPressed) moveX += 1f;

        ForwardInput = (moveZ > 0) ? moveZ : 0;

        // WICHTIG: Wenn die Kamera nach unten guckt, zeigt "forward" in den Boden.
        // Wir wollen aber nicht in den Boden laufen.
        // Deshalb nehmen wir die Richtung, machen Y platt (0) und normalisieren wieder.
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 move = right * moveX + forward * moveZ;

        // Schwerkraft
        if (!controller.isGrounded)
        {
            move.y -= gravity * Time.deltaTime;
        }

        controller.Move(move * moveSpeed * Time.deltaTime);
        
        // Escape Taste
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}