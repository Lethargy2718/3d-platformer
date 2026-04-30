using UnityEngine;
using Cinemachine;

public class FOV : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private CinemachineVirtualCamera vcam;
    private float baseFOV;
    private PlayerContext ctx;

    void Start()
    {
        baseFOV = vcam.m_Lens.FieldOfView;
        ctx = player.Context;
    }

    void LateUpdate()
    {
        if (ctx.isAiming)
        {
            AimFOV();
            return;
        }
        SpeedFOV();
    }

    private void AimFOV()
    {
        float smoothT = 1f - Mathf.Exp(-ctx.fovSmoothing * Time.deltaTime);
        float targetFOV;
        float adsT = 1f - Mathf.Exp(-ctx.adsSmoothing * Time.deltaTime);
        targetFOV = ctx.adsFOV;
        LensSettings lens = vcam.m_Lens;
        lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFOV, adsT);
        vcam.m_Lens = lens;
    }

    private void SpeedFOV()
    {
        float smoothT = 1f - Mathf.Exp(-ctx.fovSmoothing * Time.deltaTime);
        float targetFOV;
        float speed = ctx.frameVelocity.magnitude;
        float t = Mathf.Pow(Mathf.InverseLerp(0f, ctx.fovReferenceSpeed, speed), 1.25f);
        targetFOV = baseFOV + t * ctx.maxFovAmount;

        LensSettings lens2 = vcam.m_Lens;
        lens2.FieldOfView = Mathf.Lerp(lens2.FieldOfView, targetFOV, smoothT);
        vcam.m_Lens = lens2;
    }
}