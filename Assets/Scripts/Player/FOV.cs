using UnityEngine;

public class FOV : MonoBehaviour
{
    [SerializeField] private PlayerStateDriver3D player;
    public Camera cam;
    private float baseFOV;
    private PlayerContext ctx;

    void Start()
    {
        baseFOV = cam.fieldOfView;
        ctx = player.ctx;
    }

    void LateUpdate()
    {
        float smoothT = 1f - Mathf.Exp(-ctx.fovSmoothing * Time.deltaTime);

        float speed = ctx.frameVelocity.magnitude;
        float t = Mathf.Pow(Mathf.InverseLerp(0f, ctx.fovReferenceSpeed, speed), 1.25f); // TODO: add a curve
        float targetFOV = baseFOV + t * ctx.maxFovAmount;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, smoothT);
    }
}