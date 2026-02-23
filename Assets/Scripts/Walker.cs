using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Walker : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;

    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    [Header("Gravity & Steps")]
    public float gravity = 25f;
    public float maxFallSpeed = 20f;
    [Tooltip("How high a step/stair the player can walk up automatically.")]
    public float stepHeight = 0.4f;

    [Header("Look")]
    public float mouseSensitivity = 0.5f;
    [Tooltip("Lower = smoother but slightly more input lag. 0 = no smoothing.")]
    public float mouseSmoothTime = 0.03f;

    [Header("Keys")]
    public Key keyForward = Key.W;
    public Key keyBack    = Key.S;
    public Key keyLeft    = Key.A;
    public Key keyRight   = Key.D;

    public float ForwardInput { get; private set; }

    // Look
    private float rotationX;
    private float rotationY;
    private Vector2 smoothedMouseDelta;
    private Vector2 mouseSmoothVelocity;

    // Movement
    private Vector3 horizontalVelocity;
    private float   verticalVelocity;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.stepOffset = stepHeight;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        if (playerCamera != null)
            rotationX = playerCamera.localEulerAngles.x;
        rotationY = transform.localEulerAngles.y;
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null || playerCamera == null) return;

        HandleLook();
        HandleMovement();
        HandleCursor();
    }

    void HandleLook()
    {
        Vector2 rawDelta = Mouse.current.delta.ReadValue();

        // Light smoothing to take the edge off raw mouse input
        smoothedMouseDelta = mouseSmoothTime > 0f
            ? Vector2.SmoothDamp(smoothedMouseDelta, rawDelta, ref mouseSmoothVelocity, mouseSmoothTime)
            : rawDelta;

        rotationY += smoothedMouseDelta.x * mouseSensitivity;
        rotationX -= smoothedMouseDelta.y * mouseSensitivity;
        rotationX  = Mathf.Clamp(rotationX, -90f, 90f);

        // Vertical look rotates only the camera; horizontal rotates the whole body
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.localRotation    = Quaternion.Euler(0f, rotationY, 0f);
    }

    void HandleMovement()
    {
        // --- Horizontal input ---
        float inputX = 0f, inputZ = 0f;
        if (Keyboard.current[keyForward].isPressed) inputZ += 1f;
        if (Keyboard.current[keyBack].isPressed)    inputZ -= 1f;
        if (Keyboard.current[keyLeft].isPressed)    inputX -= 1f;
        if (Keyboard.current[keyRight].isPressed)   inputX += 1f;

        ForwardInput = Mathf.Max(0f, inputZ);

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right   = Vector3.ProjectOnPlane(transform.right,   Vector3.up).normalized;

        // Normalize so diagonal movement isn't faster
        Vector3 inputDir = right * inputX + forward * inputZ;
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        Vector3 targetHorizontal = inputDir * moveSpeed;

        // Smooth acceleration / deceleration
        float rate = (targetHorizontal.sqrMagnitude > 0.01f) ? acceleration : deceleration;
        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetHorizontal, rate * Time.deltaTime);

        // --- Vertical velocity / gravity ---
        if (controller.isGrounded)
        {
            // Small constant downward force keeps the controller pressed against
            // the ground so it registers as grounded on slopes and at step edges,
            // letting you walk down ramps and stairs naturally.
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
            verticalVelocity  = Mathf.Max(verticalVelocity, -maxFallSpeed);
        }

        Vector3 finalMove = horizontalVelocity;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);
    }

    void HandleCursor()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
    }
}
