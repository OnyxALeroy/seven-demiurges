using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private float rotationY;
    private float verticalVelocity;
    private float originalCharacterHeight;
    private Vector3 originalCenter;
    private float lastDashTime;
    private bool isDashing;
    private bool isCrouching;
    private Coroutine crouchRoutine = null;

    public CharacterBehaviour characterBehaviour;

    [Header("Movement Settings")]
    public float crouchSpeedMultiplier = 0.5f;
    public float sprintSpeedMultiplier = 1.5f;
    public float crouchHeightMultiplier = 0.5f;
    public float movementSpeed = 10f;
    public float rotationSpeed = 5f;
    public float jumpForce = 10f;
    public float gravity = -30f;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 2f;
    public float dashRotationMultiplier = 0.3f;
    public AnimationCurve dashCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Crouch Settings")]
    public float crouchTransitionDuration = 0.2f;
    public AnimationCurve crouchCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Layer Masks")]
    public LayerMask standUpCheckLayers = -1; // What layers to check when standing up

    public bool IsDashing => isDashing;
    public bool IsCrouching => isCrouching;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalCharacterHeight = characterController.height;
        originalCenter = characterController.center;
    }

    public SkillContext BuildSkillContext(float holdTime)
    {
        return new SkillContextBuilder()
            .WithCaster(characterBehaviour)
            .WithTarget(Vector3.zero) // FIXME: calculate the real target
            .AtPosition(characterBehaviour.transform.position)
            .InDirection(characterBehaviour.transform.forward)
            .WithHoldDuration(holdTime)
            .Build();
    }

    // --------------------------------------------------------------------------------------------
    // Movement
    // --------------------------------------------------------------------------------------------

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    public void HandleRotation(Vector2 rotationVector)
    {
        float currentRotationSpeed = rotationSpeed;

        if (isDashing)
        {
            currentRotationSpeed *= dashRotationMultiplier; // e.g., 0.3f to reduce rotation during dash
        }

        rotationY += rotationVector.x * currentRotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }

    public void HandleMovement(Vector2 movementVector, bool isSprinting, bool isCrouching)
    {
        // Calculate movement speed based on state
        float currentSpeed = movementSpeed;

        if (isCrouching && !isSprinting)
        {
            currentSpeed *= crouchSpeedMultiplier; // e.g., 0.5f for half speed
        }
        else if (isSprinting && !isCrouching)
        {
            currentSpeed *= sprintSpeedMultiplier; // e.g., 1.5f for 1.5x speed
        }
        // Note: When both are pressed, dash is triggered instead of movement modification

        Vector3 move = transform.forward * movementVector.y + transform.right * movementVector.x;
        move *= currentSpeed;

        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // small force to keep grounded
        }
        verticalVelocity += gravity * Time.deltaTime;

        // Final movement vector (horizontal + vertical)
        Vector3 velocity = move + Vector3.up * verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    // Crouching ----------------------------------------------------------------------------------

    public void StartCrouch()
    {
        if (isCrouching) return;
        isCrouching = true;
        Debug.Log("Started crouching");

        if (crouchRoutine != null) StopCoroutine(crouchRoutine);

        float targetHeight = originalCharacterHeight * crouchHeightMultiplier;
        Vector3 targetCenter = new Vector3(originalCenter.x, targetHeight / 2f, originalCenter.z);
        crouchRoutine = StartCoroutine(SmoothCrouchTransition(targetHeight, targetCenter));

        // animator.SetBool("IsCrouching", true);
    }

    public void StopCrouch()
    {
        if (!isCrouching) return;
        Debug.Log("Stopped crouching");

        if (crouchRoutine != null) StopCoroutine(crouchRoutine);

        if (CanStandUp())
        {
            isCrouching = false;
            crouchRoutine = StartCoroutine(SmoothCrouchTransition(originalCharacterHeight, originalCenter));

            // animator.SetBool("IsCrouching", false);
        }
        else
        {
            isCrouching = true;
            Debug.Log("Cannot stand up - not enough space above");
        }
    }

    private bool CanStandUp()
    {
        Vector3 rayStart = transform.position + Vector3.up * characterController.height * 0.5f;
        float checkDistance = originalCharacterHeight - characterController.height;
        return !Physics.Raycast(rayStart, Vector3.up, checkDistance + 0.1f, standUpCheckLayers);
    }

    private IEnumerator SmoothCrouchTransition(float targetHeight, Vector3 targetCenter)
    {
        float startHeight = characterController.height;
        Vector3 startCenter = characterController.center;

        float elapsed = 0f;

        while (elapsed < crouchTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / crouchTransitionDuration);
            float curvedT = crouchCurve.Evaluate(t);

            characterController.height = Mathf.Lerp(startHeight, targetHeight, curvedT);
            characterController.center = Vector3.Lerp(startCenter, targetCenter, curvedT);

            yield return null;
        }

        // Snap at end
        characterController.height = targetHeight;
        characterController.center = targetCenter;
    }

    // Sprinting ----------------------------------------------------------------------------------

    public void StartSprint()
    {
        Debug.Log("Started sprinting");

        // animator.SetBool("IsSprinting", true);
        // sprintParticles.Play();
    }

    public void StopSprint()
    {
        Debug.Log("Stopped sprinting");

        // animator.SetBool("IsSprinting", false);
        // sprintParticles.Stop();
    }

    public void PerformDash(Vector2 direction)
    {
        if (CanDash())
        {
            Debug.Log($"Dashing in direction: {direction}");

            // Calculate dash direction in world space
            Vector3 dashDirection = Vector3.zero;
            if (direction.magnitude > 0.1f)
            {
                // Dash in movement direction
                dashDirection = (transform.forward * direction.y + transform.right * direction.x).normalized;
            }
            else
            {
                // Dash forward if no input direction
                dashDirection = transform.forward;
            }

            StartCoroutine(DashCoroutine(dashDirection));
            lastDashTime = Time.time;

            // dashParticles.Play();
            // audioSource.PlayOneShot(dashSound);
        }
    }

    private bool CanDash() { return Time.time >= lastDashTime + dashCooldown; }

    private IEnumerator DashCoroutine(Vector3 dashDirection)
    {
        isDashing = true;
        float elapsed = 0f;
        Vector3 startVelocity = dashDirection * dashForce;

        while (elapsed < dashDuration)
        {
            // Apply dash movement
            float dashMultiplier = dashCurve.Evaluate(elapsed / dashDuration); // Use AnimationCurve for smooth dash
            Vector3 dashVelocity = startVelocity * dashMultiplier;

            characterController.Move(dashVelocity * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        Debug.Log("Dash completed");
    }

    // --------------------------------------------------------------------------------------------
    // Skills
    // --------------------------------------------------------------------------------------------

    public void PerformLeftClick(float heldDuration)
    {

    }
    public void HoldLeftClick()
    {

    }

    public void PerformRightClick(float heldDuration)
    {

    }

    public void HoldRightClick()
    {

    }

    public void AskForFirstSkill(float heldDuration)
    {
        SkillContext context = BuildSkillContext(heldDuration);
        characterBehaviour.AskForFirstSkill(context);
    }

    public void AskForSecondSkill(float heldDuration)
    {
        SkillContext context = BuildSkillContext(heldDuration);
        characterBehaviour.AskForSecondSkill(context);
    }

    public void AskForUltimate(float heldDuration)
    {
        SkillContext context = BuildSkillContext(heldDuration);
        characterBehaviour.AskForUltimate(context);
    }
}
