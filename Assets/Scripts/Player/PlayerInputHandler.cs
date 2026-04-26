using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public event Action OnJumpPressed;
    public event Action OnJumpReleased;
    public event Action OnDashPressed;
    public event Action OnSprintPressed;
    public event Action OnSprintReleased;
    public event Action OnUsePressed;
    public event Action<int> OnScrolled; // +1 scroll up, -1 scroll down

    private PlayerControls controls;
    private PlayerContext ctx;
    private Transform cameraTarget;

    public void Init(PlayerContext ctx, Transform cameraTarget)
    {
        this.ctx = ctx;
        this.cameraTarget = cameraTarget;
    }

    private void Awake()
    {
        controls = new PlayerControls();
        SubscribeToInputActions();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void SubscribeToInputActions()
    {
        // Move
        controls.Player.Move.performed += c => ctx.moveInput = c.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => ctx.moveInput = Vector2.zero;

        // Jump
        controls.Player.Jump.performed += _ =>
        {
            ctx.jumpPressed = true;
            ctx.jumpHeld = true;
            ctx.timeJumpWasPressed = ctx.time;
            ctx.hasBufferedJump = true;
            OnJumpPressed?.Invoke();
        };
        controls.Player.Jump.canceled += _ =>
        {
            ctx.jumpHeld = false;
            ctx.jumpPressed = false;
            OnJumpReleased?.Invoke();
        };

        // Dash 
        controls.Player.Dash.performed += _ =>
        {
            ctx.dashPressed = true;
            ctx.timeDashWasPressed = ctx.time;
            ctx.hasBufferedDash = true;
            ctx.dashDirection = ctx.moveInput.sqrMagnitude > 0.01f
                ? (cameraTarget.forward * ctx.moveInput.y
                 + cameraTarget.right * ctx.moveInput.x).normalized
                : cameraTarget.forward;
            OnDashPressed?.Invoke();
        };

        // Sprint 
        controls.Player.Sprint.performed += _ => { ctx.sprintHeld = true; OnSprintPressed?.Invoke(); };
        controls.Player.Sprint.canceled += _ => { ctx.sprintHeld = false; OnSprintReleased?.Invoke(); };

        // Use 
        controls.Player.Use.performed += _ => OnUsePressed?.Invoke();

        // Scroll 
        controls.Player.Scroll.performed += c =>
        {
            float delta = c.ReadValue<Vector2>().y;
            if (delta != 0f)
                OnScrolled?.Invoke(delta > 0f ? -1 : 1);
        };
    }
}