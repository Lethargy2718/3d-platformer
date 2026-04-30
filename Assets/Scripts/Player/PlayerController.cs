using System;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerCollisionSensor))]
[RequireComponent(typeof(PlayerLookHandler))]
[RequireComponent(typeof(PlayerInventoryController))]
public class PlayerController : MonoBehaviour
{
    public Inventory Inventory => InventoryController.Inventory;

    [SerializeField] private PlayerContext ctx = new PlayerContext();
    public PlayerContext Context => ctx;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI stateText;

    [Header("Camera")]
    [SerializeField] private Transform cameraTarget;

    [Header("Throwing")]
    [SerializeField] private float throwForce = 12f;
    [SerializeField] private Transform aimOrigin;
    public Transform AimOrigin => aimOrigin;


    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerCollisionSensor CollisionSensor { get; private set; }
    public PlayerLookHandler LookHandler { get; private set; }
    public PlayerInventoryController InventoryController { get; private set; }

    private StateMachine machine;
    private string lastStatePath;
    private Camera cam;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var rb = GetComponent<Rigidbody>();
        var col = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.useGravity = false;

        Context.rb = rb;
        Context.col = col;
        Context.transform = transform;

        InputHandler = GetComponent<PlayerInputHandler>();
        CollisionSensor = GetComponent<PlayerCollisionSensor>();
        LookHandler = GetComponent<PlayerLookHandler>();
        InventoryController = GetComponent<PlayerInventoryController>();

        // NOTE: before inventory
        InputHandler.Init(Context, cameraTarget);
        CollisionSensor.Init(Context, col);
        LookHandler.Init(Context, rb);

        // State machine
        var root = new PlayerRoot(null, Context);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();

        cam = Camera.main;
    }

    private void Start()
    {
        InventoryController.Init(this, InputHandler);
    }

    private void Update()
    {
        Context.time += Time.deltaTime;
        CalculateMoveDirection();
        machine.Tick(Time.deltaTime);

        Context.jumpPressed = false;
        Context.dashPressed = false;
    }

    private void FixedUpdate()
    {
        CollisionSensor.CheckCollisions();
        LookHandler.ApplyLook();
        Context.rb.linearVelocity = Context.frameVelocity;
        machine.FixedTick(Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision) => machine.HandleCollision(collision);

    private void LateUpdate() => UpdateStateText();

    private void CalculateMoveDirection()
    {
        if (Context.moveInput.sqrMagnitude < 0.001f)
        {
            Context.moveDirection = Vector3.zero;
            return;
        }

        var yawRot = Quaternion.Euler(0f, Context.yaw, 0f);
        Context.moveDirection = (yawRot * new Vector3(Context.moveInput.x, 0f, Context.moveInput.y)).normalized;

        if (Context.moveInput.x != 0f) Context.currentDirection.x = Mathf.Sign(Context.moveInput.x);
        if (Context.moveInput.y != 0f) Context.currentDirection.y = Mathf.Sign(Context.moveInput.y);
    }

    private void UpdateStateText()
    {
        if (stateText == null) return;

        var path = string.Join("\n > ",
            machine.Root.Leaf().PathToRoot().Reverse().Select(n => n.GetType().Name));

        if (path == lastStatePath) return;
        lastStatePath = path;
        stateText.text = path;
    }

    // TODO: serialize fields
    public Vector3 GetAimDirection(Vector3 aimOrigin)
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint = ray.GetPoint(100f);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Dot((hit.point - aimOrigin), cam.transform.forward) >= 0.1f)
            {
                targetPoint = hit.point;
            }
        }

        Debug.DrawLine(aimOrigin, targetPoint, Color.green, 0.5f);
        return (targetPoint - aimOrigin).normalized;
    }
}