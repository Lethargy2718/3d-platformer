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
    public Inventory Inventory => inventoryController.Inventory;

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


    private PlayerInputHandler inputHandler;
    private PlayerCollisionSensor collisionSensor;
    private PlayerLookHandler lookHandler;
    private PlayerInventoryController inventoryController;

    private StateMachine machine;
    private string lastStatePath;

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

        inputHandler = GetComponent<PlayerInputHandler>();
        collisionSensor = GetComponent<PlayerCollisionSensor>();
        lookHandler = GetComponent<PlayerLookHandler>();
        inventoryController = GetComponent<PlayerInventoryController>();

        // NOTE: before inventory
        inputHandler.Init(Context, cameraTarget);
        collisionSensor.Init(Context, col);
        lookHandler.Init(Context, rb);

        // State machine
        var root = new PlayerRoot(null, Context);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();
    }

    private void Start()
    {
        inventoryController.Init(this, inputHandler);
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
        collisionSensor.CheckCollisions();
        lookHandler.ApplyLook();
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
    public Vector3 GetAimDirection()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 origin = aimOrigin.position;

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            Vector3 toHit = hit.point - origin;

            // Corrects reversed shots
            if (Vector3.Dot(toHit, cam.transform.forward) < 0.1f)
            {
                Vector3 fallback = ray.GetPoint(25f);
                return (fallback - origin).normalized;
            }

            return toHit.normalized;
        }

        return (ray.GetPoint(25f) - origin).normalized;
    }
}