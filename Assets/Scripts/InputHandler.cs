using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerController characterController;
    public InputActionAsset inputActions;

    private float leftPressTime;
    private float rightPressTime;
    private float firstSkillPressTime;
    private float secondSkillPressTime;
    private float ultimatePressTime;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction fireLeftAction;
    private InputAction fireRightAction;
    private InputAction firstSkill;
    private InputAction secondSkill;
    private InputAction ultimateSkill;

    // New input actions
    private InputAction crouchAction;
    private InputAction sprintAction;

    // State tracking for dash
    private bool isSprinting;
    private bool isCrouching;

    public float LeftPressTime => leftPressTime;
    public float RightPressTime => rightPressTime;
    public bool IsSprinting => isSprinting;
    public bool IsCrouching => isCrouching;

    private void OnEnable()
    {
        var playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
        jumpAction = playerMap.FindAction("Jump");
        fireLeftAction = playerMap.FindAction("Attack");
        fireRightAction = playerMap.FindAction("Target");
        firstSkill = playerMap.FindAction("FirstSkill");
        secondSkill = playerMap.FindAction("SecondSkill");
        ultimateSkill = playerMap.FindAction("Ultimate");
        crouchAction = playerMap.FindAction("Crouch");
        sprintAction = playerMap.FindAction("Sprint");

        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        fireLeftAction.Enable();
        fireRightAction.Enable();
        firstSkill.Enable();
        secondSkill.Enable();
        ultimateSkill.Enable();
        crouchAction.Enable();
        sprintAction.Enable();

        jumpAction.performed += OnJumpPerformed;
        fireLeftAction.canceled += OnLeftRelease;
        fireRightAction.canceled += OnRightRelease;
        firstSkill.canceled += OnFirstSkillUsed;
        secondSkill.canceled += OnSecondSkillUsed;
        ultimateSkill.canceled += OnUltimateUsed;

        // Subscribe to new input events
        crouchAction.started += OnCrouchStarted;
        crouchAction.canceled += OnCrouchCanceled;
        sprintAction.started += OnSprintStarted;
        sprintAction.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        fireLeftAction.Disable();
        fireRightAction.Disable();
        firstSkill.Disable();
        secondSkill.Disable();
        ultimateSkill.Disable();
        crouchAction.Disable();
        sprintAction.Disable();

        // Unsubscribe from events to prevent memory leaks
        jumpAction.performed -= OnJumpPerformed;
        fireLeftAction.canceled -= OnLeftRelease;
        fireRightAction.canceled -= OnRightRelease;
        firstSkill.canceled -= OnFirstSkillUsed;
        secondSkill.canceled -= OnSecondSkillUsed;
        ultimateSkill.canceled -= OnUltimateUsed;
        crouchAction.started -= OnCrouchStarted;
        crouchAction.canceled -= OnCrouchCanceled;
        sprintAction.started -= OnSprintStarted;
        sprintAction.canceled -= OnSprintCanceled;
    }

    private void Update()
    {
        Vector2 movementVector = moveAction.ReadValue<Vector2>();
        characterController.HandleMovement(movementVector, isSprinting, isCrouching);

        Vector2 lookVector = lookAction.ReadValue<Vector2>();
        characterController.HandleRotation(lookVector);

        if (fireLeftAction.IsPressed()) { leftPressTime += Time.deltaTime; }
        if (fireRightAction.IsPressed()) { rightPressTime += Time.deltaTime; }
        if (firstSkill.IsPressed()) { firstSkillPressTime += Time.deltaTime; }
        if (secondSkill.IsPressed()) { secondSkillPressTime += Time.deltaTime; }
        if (ultimateSkill.IsPressed()) { ultimatePressTime += Time.deltaTime; }
    }

    // --------------------------------------------------------------------------------------------
    // Original Methods
    // --------------------------------------------------------------------------------------------

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        characterController.Jump();
    }

    private void OnLeftRelease(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Left released after {leftPressTime} seconds");

        characterController.PerformLeftClick(leftPressTime);
        leftPressTime = 0f;
    }

    private void OnRightRelease(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Right released after {rightPressTime} seconds");

        characterController.PerformRightClick(rightPressTime);
        rightPressTime = 0f;
    }

    private void OnFirstSkillUsed(InputAction.CallbackContext ctx)
    {
        characterController.AskForFirstSkill(firstSkillPressTime);
        firstSkillPressTime = 0f;
    }

    private void OnSecondSkillUsed(InputAction.CallbackContext ctx)
    {
        characterController.AskForSecondSkill(secondSkillPressTime);
        secondSkillPressTime = 0f;
    }

    private void OnUltimateUsed(InputAction.CallbackContext ctx)
    {
        characterController.AskForUltimate(ultimatePressTime);
        ultimatePressTime = 0f;
    }

    // --------------------------------------------------------------------------------------------
    // New Movement Methods
    // --------------------------------------------------------------------------------------------

    private void OnCrouchStarted(InputAction.CallbackContext ctx)
    {
        isCrouching = true;

        // Check for dash (sprint + crouch simultaneously)
        if (isSprinting)
        {
            PerformDash();
        }
        else
        {
            characterController.StartCrouch();
        }
    }

    private void OnCrouchCanceled(InputAction.CallbackContext ctx)
    {
        isCrouching = false;
        characterController.StopCrouch();
    }

    private void OnSprintStarted(InputAction.CallbackContext ctx)
    {
        isSprinting = true;

        // Check for dash (sprint + crouch simultaneously)
        if (isCrouching)
        {
            PerformDash();
        }
        else
        {
            characterController.StartSprint();
        }
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        isSprinting = false;
        characterController.StopSprint();
    }

    private void PerformDash()
    {
        Debug.Log("Dash performed!");

        // Get current movement direction for dash
        Vector2 movementVector = moveAction.ReadValue<Vector2>();
        characterController.PerformDash(movementVector);
    }
}
