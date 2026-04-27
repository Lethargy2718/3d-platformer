using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera thirdPersonCam;
    [SerializeField] private CinemachineVirtualCamera firstPersonCam;
    [SerializeField] private Camera virtualModelCam;
    [SerializeField] private LayerMask worldModelLayer;

    private PlayerControls controls;
    private bool isFirstPerson = false;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SwitchCamera.performed += _ => Switch();
    }

    private void Start()
    {
        UpdateCams();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Switch()
    {
        isFirstPerson = !isFirstPerson;
        UpdateCams();
    }
    
    private void UpdateCams()
    {
        UpdatePriorities();
        UpdateVirtualModelCam();
        UpdateMainCamMask();
    }

    private void UpdatePriorities()
    {
        thirdPersonCam.Priority = isFirstPerson ? 0 : 10;
        firstPersonCam.Priority = isFirstPerson ? 10 : 0;
    }

    private void UpdateVirtualModelCam()
    {
        virtualModelCam.gameObject.SetActive(isFirstPerson);
    }

    // Only show world model in 3rd person
    private void UpdateMainCamMask()
    {
        var mainCam = Camera.main;
        if (isFirstPerson)
            mainCam.cullingMask &= ~worldModelLayer;
        else
            mainCam.cullingMask |= worldModelLayer;
    }
}