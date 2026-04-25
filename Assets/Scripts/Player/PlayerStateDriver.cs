using System;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerStateDriver3D : MonoBehaviour
{
    public PlayerContext ctx = new PlayerContext();

    // TODO: remove
    public Item placeholderItem = null;

    public Inventory inventory = new(8); // TODO: serialize size

    private int currentInventoryItemIdx = 0;
    private Item CurrentItem => inventory.Slots[currentInventoryItemIdx]?.item;

    [Header("References")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private CameraRig cameraRig;

    [Header("Collision")]
    [SerializeField] private float groundCheckDistance = 0.05f;
    [SerializeField] private float ceilingCheckDistance = 0.05f;
    [SerializeField] private LayerMask excludeFromCollisions;

    private PlayerControls controls;
    private StateMachine machine;
    private string lastStatePath;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ctx.rb = GetComponent<Rigidbody>();
        ctx.col = GetComponent<CapsuleCollider>();
        ctx.transform = transform;

        ctx.rb.freezeRotation = true;
        ctx.rb.useGravity = false;

        controls = new PlayerControls();
        SubscribeToInputEvents();

        var root = new PlayerRoot(null, ctx);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();
    }

    private void Start()
    {
        inventory.AddItem(placeholderItem, 1);
    }

    private void Update()
    {
        ctx.time += Time.deltaTime;
        CalculateMoveDirection();
        machine.Tick(Time.deltaTime);
        ctx.jumpPressed = false;
        ctx.dashPressed = false;
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        ApplyLook();
        ctx.rb.linearVelocity = ctx.frameVelocity;
        machine.FixedTick(Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        machine.HandleCollision(collision);
    }

    private void LateUpdate()
    {
        if (stateText == null) return;
        var path = StatePath(machine.Root.Leaf());
        if (path == lastStatePath) return;
        lastStatePath = path;
        stateText.text = path;

        UpdateItemText();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void SubscribeToInputEvents()
    {
        // Move
        controls.Player.Move.performed += ctx2 => ctx.moveInput = ctx2.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => ctx.moveInput = Vector2.zero;

        // Jump
        controls.Player.Jump.performed += _ =>
        {
            ctx.jumpPressed = true;
            ctx.jumpHeld = true;
            ctx.timeJumpWasPressed = ctx.time;
            ctx.hasBufferedJump = true;
        };
        controls.Player.Jump.canceled += _ =>
        {
            ctx.jumpHeld = false;
            ctx.jumpPressed = false;
        };

        controls.Player.Dash.performed += _ =>
        {
            ctx.dashPressed = true;
            ctx.timeDashWasPressed = ctx.time;
            ctx.hasBufferedDash = true;
            ctx.dashDirection = ctx.moveInput.sqrMagnitude > 0.01f
                ? (cameraTarget.forward * ctx.moveInput.y + cameraTarget.right * ctx.moveInput.x).normalized
                : cameraTarget.forward;
        };

        // Sprint
        controls.Player.Sprint.performed += _ =>
        {
            ctx.sprintHeld = true;
        };
        controls.Player.Sprint.canceled += _ =>
        {
            ctx.sprintHeld = false;
        };

        // Use item
        controls.Player.Use.performed += _ =>
        {
            UseCurrentItem();
        };
    }

    private void CalculateMoveDirection()
    {
        if (ctx.moveInput.sqrMagnitude < 0.001f)
        {
            ctx.moveDirection = Vector3.zero;
            return;
        }

        Quaternion yawRot = Quaternion.Euler(0f, ctx.yaw, 0f);
        ctx.moveDirection = yawRot * new Vector3(ctx.moveInput.x, 0f, ctx.moveInput.y);
        ctx.moveDirection = ctx.moveDirection.normalized;

        if (ctx.moveInput.x != 0f) ctx.currentDirection.x = Mathf.Sign(ctx.moveInput.x);
        if (ctx.moveInput.y != 0f) ctx.currentDirection.y = Mathf.Sign(ctx.moveInput.y);
    }

    private void ApplyLook()
    {
        // TODO: either remove ctx.yaw or change it with an event
        ctx.yaw = cameraRig.Yaw;

        if (cameraTarget != null)
            ctx.rb.MoveRotation(Quaternion.Euler(0f, ctx.yaw, 0f));
    }

    private void CheckCollisions()
    {
        Vector3 center = ctx.col.bounds.center;
        float radius = ctx.col.radius;
        float halfH = ctx.col.height * 0.5f - radius;

        bool groundHit = Physics.SphereCast(center, radius, Vector3.down, out _, halfH + groundCheckDistance, ~excludeFromCollisions, QueryTriggerInteraction.Ignore);
        bool ceilingHit = Physics.SphereCast(center, radius, Vector3.up, out _, halfH + ceilingCheckDistance, ~excludeFromCollisions, QueryTriggerInteraction.Ignore);

        if (ceilingHit)
            ctx.frameVelocity.y = Mathf.Min(0f, ctx.frameVelocity.y);

        ctx.grounded = groundHit;
        ctx.ceilingHit = ceilingHit;
    }

    private static string StatePath(State s) =>
        string.Join("\n > ", s.PathToRoot().Reverse().Select(n => n.GetType().Name));

    public bool Throw(ThrowableItem throwable)
    {
        // TODO: throw he throwable.prefab instead of throwing an exception
        throw new NotImplementedException();
    }

    private void UseCurrentItem()
    {
        if (CurrentItem == null) return;

        if (CurrentItem.Use(this))
        {
            // TODO: uncomment after implementing RemoveItem(itemIdx, count)
            // inventory.RemoveItem(currentItemIdx, 1)
        }
    }

    private void UpdateItemText()
    {
        itemText.text = CurrentItem != null ? CurrentItem.itemName : "No item";
    }
}