using UnityEngine;

public class ThirdPersonCameraRig : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float sensitivity = 0.2f;
    [SerializeField] private float pitchMin = -80f;
    [SerializeField] private float pitchMax = 80f;

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }

    private Vector2 lookInput;
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += _ => lookInput = Vector2.zero;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void LateUpdate()
    {
        Yaw += lookInput.x * sensitivity;
        Pitch -= lookInput.y * sensitivity;
        Pitch = Mathf.Clamp(Pitch, pitchMin, pitchMax);

        transform.position = target.position;
        transform.rotation = Quaternion.Euler(Pitch, Yaw, 0f);
    }
}