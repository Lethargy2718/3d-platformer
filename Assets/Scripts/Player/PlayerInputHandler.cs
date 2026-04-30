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
    public event Action DropPressed;
    public event Action DropAllPressed;
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
        var p = controls.Player;

        // Move
        p.Move.performed += c => ctx.moveInput = c.ReadValue<Vector2>();
        p.Move.canceled += _ => ctx.moveInput = Vector2.zero;

        // Jump
        p.Jump.performed += _ =>
        {
            ctx.jumpPressed = true;
            ctx.jumpHeld = true;
            ctx.timeJumpWasPressed = ctx.time;
            ctx.hasBufferedJump = true;
            JumpPressed?.Invoke();
        };
        p.Jump.canceled += _ =>
        {
            ctx.jumpHeld = false;
            ctx.jumpPressed = false;
            JumpReleased?.Invoke();
        };

        // Dash 
        p.Dash.performed += _ =>
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
        p.Sprint.performed += _ => { ctx.sprintHeld = true; SprintPressed?.Invoke(); };
        p.Sprint.canceled += _ => { ctx.sprintHeld = false; SprintReleased?.Invoke(); };

        // Use 
        p.Use.performed += _ => UsePressed?.Invoke();
        p.Use.canceled += _ => UseCanceled?.Invoke();

        // Scroll 
        p.Scroll.performed += c =>
        {
            float delta = c.ReadValue<Vector2>().y;
            if (delta != 0f)
                Scrolled?.Invoke(delta > 0f ? -1 : 1);
        };

        // Drop
        p.Drop.performed += _ =>
        {
            DropPressed?.Invoke();
        };

        // Drop all
        p.DropAll.performed += _ =>
        {
            DropAllPressed?.Invoke();
        };

        // Aim
        p.Aim.performed += _ => ctx.isAiming = true;
        p.Aim.canceled += _ => ctx.isAiming = false;
    }
}