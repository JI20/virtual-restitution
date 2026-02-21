using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Walker : MonoBehaviour
{
    [Header("Referenzen")]
    public Transform playerCamera; // NEU: Wir brauchen die Kamera hier!

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

    public float ForwardInput { get; private set; }

    private float rotationX = 0f; 
    private float rotationY = 0f; 

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera != null)
        {
            rotationX = playerCamera.localEulerAngles.x;
        }
        rotationY = transform.localEulerAngles.y;
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null || playerCamera == null) return;

        // --- 1. Maus Input ---
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        rotationY += mouseDelta.x * mouseSensitivity; 
        rotationX -= mouseDelta.y * mouseSensitivity; 
        
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // DIE WICHTIGE ANPASSUNG: 
        // Hoch/Runter dreht NUR die Kamera. Links/Rechts dreht den GANZEN Spieler.
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.localRotation = Quaternion.Euler(0, rotationY, 0);

        // --- 2. Bewegung ---
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current[keyForward].isPressed) moveZ += 1f;
        if (Keyboard.current[keyBack].isPressed) moveZ -= 1f;
        if (Keyboard.current[keyLeft].isPressed) moveX -= 1f;
        if (Keyboard.current[keyRight].isPressed) moveX += 1f;

        ForwardInput = (moveZ > 0) ? moveZ : 0;

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