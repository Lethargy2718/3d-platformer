using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public event Action JumpPressed;
    public event Action JumpReleased;
    public event Action DashPressed;
    public event Action SprintPressed;
    public event Action SprintReleased;
    public event Action UsePressed;
    public event Action UseCanceled;
    public event Action<int> Scrolled; // +1 scroll up, -1 scroll down

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
            JumpPressed?.Invoke();
        };
        controls.Player.Jump.canceled += _ =>
        {
            ctx.jumpHeld = false;
            ctx.jumpPressed = false;
            JumpReleased?.Invoke();
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
            DashPressed?.Invoke();
        };

        // Sprint 
        controls.Player.Sprint.performed += _ => { ctx.sprintHeld = true; SprintPressed?.Invoke(); };
        controls.Player.Sprint.canceled += _ => { ctx.sprintHeld = false; SprintReleased?.Invoke(); };

        // Use 
        controls.Player.Use.performed += _ => UsePressed?.Invoke();
        controls.Player.Use.canceled += _ => UseCanceled?.Invoke();

        // Scroll 
        controls.Player.Scroll.performed += c =>
        {
            float delta = c.ReadValue<Vector2>().y;
            if (delta != 0f)
                Scrolled?.Invoke(delta > 0f ? -1 : 1);
        };
    }
}