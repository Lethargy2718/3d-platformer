using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera thirdPersonCam;
    [SerializeField] private CinemachineVirtualCamera firstPersonCam;

    private PlayerControls controls;
    private bool isFirstPerson = false;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SwitchCamera.performed += _ => Switch();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Switch()
    {
        // TODO: check out alternatives to hardcoding priorities
        isFirstPerson = !isFirstPerson;
        thirdPersonCam.Priority = isFirstPerson ? 0 : 10;
        firstPersonCam.Priority = isFirstPerson ? 10 : 0;
    }
}