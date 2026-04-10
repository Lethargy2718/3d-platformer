using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [SerializeField] private PlayerStateDriver3D player;
    public Transform cam;

    private Vector3 baseLocalPos;
    private float bobTime;
    private PlayerContext ctx;

    void Start()
    {
        // assuming it's only set once at the start. if it ever changes somewhere else, i'll need a centralized manager
        baseLocalPos = cam.localPosition;

        // TODO: make context a scriptable object and supply it from the inspector instead.
        ctx = player.ctx;
    }

    void LateUpdate()
    {
        ctx.currentBobStrength = Mathf.Lerp(
            ctx.currentBobStrength,
            ctx.targetBobStrength,
            Time.deltaTime * ctx.bobSmoothing
        );

        // Return to original position
        if (ctx.currentBobStrength < ctx.bobStopThreshold)
        {
            cam.localPosition = Vector3.Lerp(
                cam.localPosition,
                baseLocalPos,
                Time.deltaTime * ctx.bobSmoothing
            );
            return;
        }

        Vector3 horizontalVel = new Vector3(
            ctx.frameVelocity.x, 0f, ctx.frameVelocity.z
        );

        float speed = horizontalVel.magnitude;

        bobTime += Time.deltaTime * ctx.bobFrequency * speed;

        Vector3 bobOffset = new Vector3(
            Mathf.Cos(bobTime * ctx.bobXMultiplier),
            Mathf.Sin(bobTime),
            0f
        ) * ctx.bobAmplitude * ctx.currentBobStrength;

        cam.localPosition = Vector3.Lerp(
            cam.localPosition,
            baseLocalPos + bobOffset,
            Time.deltaTime * ctx.bobCameraSmoothing
        );
    }
}