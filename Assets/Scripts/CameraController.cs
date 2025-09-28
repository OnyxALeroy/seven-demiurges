using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerBody;       // Reference to the player transform
    public float mouseSensitivity = 100f;

    [Header("Camera Settings")]
    public float dashCameraShake = 2f;     // Camera shake intensity during dash
    public float crouchCameraOffset = -0.5f; // How much to lower camera when crouching

    private float xRotation = 0f;      // Pitch (looking up/down)
    private Vector3 originalLocalPosition;
    private bool isShaking = false;

    // References to other components
    private PlayerController playerController;

    private void Start()
    {
        // Lock and hide cursor for FPS style
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Store original camera position
        originalLocalPosition = transform.localPosition;

        // Get player controller reference
        playerController = playerBody.GetComponent<PlayerController>();
    }

    public void HandleCameraRotation(Vector2 lookVector)
    {
        // Apply sensitivity
        float mouseX = lookVector.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookVector.y * mouseSensitivity * Time.deltaTime;

        // Optional: Reduce sensitivity during dash for better control
        if (playerController != null && playerController.IsDashing)
        {
            mouseX *= 0.3f; // Reduce horizontal sensitivity during dash
            mouseY *= 0.3f; // Reduce vertical sensitivity during dash
        }

        // Vertical rotation (clamped so player can't flip backwards)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotation to the camera (pitch)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply rotation to the player body (yaw)
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();

        // Handle dash camera shake
        if (playerController != null && playerController.IsDashing && !isShaking)
        {
            StartCoroutine(DashCameraShake());
        }
    }

    private void UpdateCameraPosition()
    {
        Vector3 targetPosition = originalLocalPosition;

        // Adjust camera height when crouching
        if (playerController != null && playerController.IsCrouching)
        {
            targetPosition.y += crouchCameraOffset;
        }

        // Smooth transition to target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 5f);
    }

    private System.Collections.IEnumerator DashCameraShake()
    {
        isShaking = true;
        float elapsed = 0f;
        float shakeDuration = 0.2f; // Short shake duration

        while (elapsed < shakeDuration && playerController.IsDashing)
        {
            // Generate random shake offset
            float shakeAmount = dashCameraShake * (1f - elapsed / shakeDuration); // Fade out over time
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeAmount, shakeAmount) * 0.01f,
                Random.Range(-shakeAmount, shakeAmount) * 0.01f,
                0f
            );

            // Apply shake to rotation instead of position for better FPS feel
            Quaternion shakeRotation = Quaternion.Euler(
                shakeOffset.y * 100f,
                shakeOffset.x * 100f,
                0f
            );

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f) * shakeRotation;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to normal rotation
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        isShaking = false;
    }

    // Method to reset camera shake if needed
    public void StopCameraShake()
    {
        StopAllCoroutines();
        isShaking = false;
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}