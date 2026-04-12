using UnityEngine;
using Cinemachine;

public class FOV : MonoBehaviour
{
    [SerializeField] private PlayerStateDriver3D player;
    [SerializeField] private CinemachineVirtualCamera vcam;
    private float baseFOV;
    private PlayerContext ctx;

    void Start()
    {
        baseFOV = vcam.m_Lens.FieldOfView;
        ctx = player.ctx;
    }

    void LateUpdate()
    {
        float smoothT = 1f - Mathf.Exp(-ctx.fovSmoothing * Time.deltaTime);

        float speed = ctx.frameVelocity.magnitude;
        float t = Mathf.Pow(Mathf.InverseLerp(0f, ctx.fovReferenceSpeed, speed), 1.25f); // TODO: add a curve
        float targetFOV = baseFOV + t * ctx.maxFovAmount;

        LensSettings lens = vcam.m_Lens;
        lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFOV, smoothT);
        vcam.m_Lens = lens;
    }
}