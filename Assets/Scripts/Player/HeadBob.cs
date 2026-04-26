using System.Xml;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    public Transform cam;

    private Vector3 baseLocalPos;
    private float bobTime;
    private PlayerContext ctx;

    private void Start()
    {
        // assuming it's only set once at the start. if it ever changes somewhere else, i'll need a centralized manager
        baseLocalPos = cam.localPosition;

        // TODO: make context a scriptable object and supply it from the inspector instead.
        ctx = player.Context;
    }

    private void LateUpdate()
    {
        float cameraT = 1f - Mathf.Exp(-ctx.bobCameraSmoothing * Time.deltaTime);

        float speed = new Vector2(ctx.frameVelocity.x, ctx.frameVelocity.z).magnitude;
        bool isDashing = ctx.time < ctx.timeDashWasPressed + ctx.dashDuration;
        float strength = ctx.grounded && !isDashing ? Mathf.InverseLerp(0f, ctx.CurrentMaxSpeed, speed) : 0f;

        if (strength < 0.01f)
        {
            cam.localPosition = Vector3.Lerp(cam.localPosition, baseLocalPos, cameraT);
            return;
        }

        bobTime += Time.deltaTime * ctx.bobFrequency * speed;
        Vector3 bobOffset = ctx.bobAmplitude * strength * new Vector3(
            Mathf.Cos(bobTime * ctx.bobXMultiplier),
            Mathf.Sin(bobTime),
            0f
        );

        cam.localPosition = Vector3.Lerp(cam.localPosition, baseLocalPos + bobOffset, cameraT);
    }
}