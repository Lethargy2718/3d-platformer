using System;
using UnityEngine;

[Serializable]
public class PlayerContext
{
    // Components
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CapsuleCollider col;
    [HideInInspector] public Transform transform;

    // Time
    [HideInInspector] public float time;

    // Input
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 lookInput;
    [HideInInspector] public bool jumpPressed;
    [HideInInspector] public bool jumpHeld;
    [HideInInspector] public bool dashPressed;
    [HideInInspector] public bool sprintHeld;

    // Move
    [HideInInspector] public Vector3 frameVelocity;
    [HideInInspector] public Vector3 moveDirection;
    [HideInInspector] public Vector2 currentDirection;

    // Collision
    [HideInInspector] public bool grounded;
    [HideInInspector] public float lastGroundedTime = float.MinValue;
    [HideInInspector] public bool ceilingHit;
    [HideInInspector] public bool forwardHit;

    // Jump
    [HideInInspector] public bool hasBufferedJump;
    [HideInInspector] public bool endedJumpEarly;
    [HideInInspector] public bool coyoteUsable;
    [HideInInspector] public float timeJumpWasPressed = float.MinValue;

    // Dash
    [HideInInspector] public bool hasBufferedDash;
    [HideInInspector] public float timeDashWasPressed = float.MinValue;
    [HideInInspector] public float timeDashEnded = float.MinValue;
    [HideInInspector] public Vector3 dashDirection;
    [HideInInspector] public bool hasAirDashed;

    // Look
    [HideInInspector] public float yaw;
    [HideInInspector] public Vector3 aimDirection;

    [Header("Horizontal Movement")]
    public float maxWalkSpeed = 6f;
    public float sprintMultiplier = 1.6f;
    public float groundAcceleration = 80f;
    public float groundFriction = 10f;
    public float airAcceleration = 10f;
    public float airFriction = 10f;

    [Header("Head Bob")]
    public float bobFrequency = 10f;
    public float bobAmplitude = 0.05f;
    public float bobCameraSmoothing = 12f;
    public float bobXMultiplier = 0.5f;

    [Header("FOV")]
    public float fovSmoothing = 8f;
    public float maxFovAmount = 15f;
    public float fovReferenceSpeed = 40f;

    [Header("Jump")]
    public float jumpHeight = 2f;
    public float coyoteTime = 0.15f;
    public float jumpBuffer = 0.15f;
    public float jumpEndEarlyGravityModifier = 3f;

    [Header("Gravity")]
    public float fallAcceleration = 35f;
    public float maxFallSpeed = 30f;
    public float groundingForce = -2f;

    [Header("Dash")]
    public float dashSpeed = 14f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 1f;
    public float dashBuffer = 0.15f;
    public float maxWallNormalY = 0.9f;

    public bool CanUseBufferedJump => hasBufferedJump && time < timeJumpWasPressed + jumpBuffer;

    public bool CanUseCoyote => coyoteUsable && !grounded && time < lastGroundedTime + coyoteTime;

    public bool CanUseBufferedDash =>
        hasBufferedDash
        && time < timeDashWasPressed + dashBuffer
        && time > timeDashEnded + dashCooldown
        && !(hasAirDashed && !grounded);

    public float CurrentMaxSpeed => sprintHeld ? maxWalkSpeed * sprintMultiplier : maxWalkSpeed;

    // Events
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    public event Action Dashed;
    public event Action DashEnded;

    public void InvokeJumped() => Jumped?.Invoke();
    public void InvokeDashed() => Dashed?.Invoke();
    public void InvokeDashEnded() => DashEnded?.Invoke();
    public void InvokeGroundedChanged(bool g, float v) => GroundedChanged?.Invoke(g, v);
}