using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float gravity = -9.81f; // keeps the player grounded on slopes/steps, not for jumping

    [Header("Mouse Look")]
    public Transform cameraTransform; // the Camera, as a child of this object
    public float mouseSensitivity = 2f;
    public float minLookAngle = -80f;
    public float maxLookAngle = 80f;

    [Header("Cursor")]
    public bool lockCursorOnStart = true;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (lockCursorOnStart)
            SetCursorLocked(true);
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
        HandleCursorToggle();
    }

    void HandleMovement()
    {
        if (Keyboard.current == null)
            return;

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ += 1f;
        if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
        if (Keyboard.current.dKey.isPressed) moveX += 1f;
        if (Keyboard.current.aKey.isPressed) moveX -= 1f;

        Vector3 move = (transform.right * moveX + transform.forward * moveZ);

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        move *= moveSpeed;

        // Simple gravity so the player stays grounded on uneven floors/ramps.
        // No jump input at all, by design.
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    void HandleLook()
    {
        if (Mouse.current == null || cameraTransform == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // Yaw - rotate the whole player body left/right
        transform.Rotate(Vector3.up * mouseX);

        // Pitch - rotate only the camera up/down, clamped so you can't flip over
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    void HandleCursorToggle()
    {
        // Press Escape to free the cursor (e.g. to click UI), click back into
        // the game window to re-lock it.
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SetCursorLocked(false);

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            SetCursorLocked(true);
    }

    void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}