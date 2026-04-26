using UnityEngine;

public class PlayerLookHandler : MonoBehaviour
{
    [SerializeField] private CameraRig cameraRig;

    private PlayerContext ctx;
    private Rigidbody rb;

    public void Init(PlayerContext ctx, Rigidbody rb)
    {
        this.ctx = ctx;
        this.rb = rb;
    }

    // NOTE: call in FixedUpdate()
    public void ApplyLook()
    {
        ctx.yaw = cameraRig.Yaw;
        rb.MoveRotation(Quaternion.Euler(0f, ctx.yaw, 0f));
    }
}