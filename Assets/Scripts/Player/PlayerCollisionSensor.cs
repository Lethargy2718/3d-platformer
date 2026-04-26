using UnityEngine;

public class PlayerCollisionSensor : MonoBehaviour
{
    [SerializeField] private float groundCheckDistance = 0.05f;
    [SerializeField] private float ceilingCheckDistance = 0.05f;
    [SerializeField] private LayerMask excludeFromCollisions;

    private PlayerContext ctx;
    private CapsuleCollider col;

    public void Init(PlayerContext ctx, CapsuleCollider col)
    {
        this.ctx = ctx;
        this.col = col;
    }

    public void CheckCollisions()
    {
        Vector3 center = col.bounds.center;
        float radius = col.radius;
        float halfH = col.height * 0.5f - radius;
        int mask = ~excludeFromCollisions;

        bool groundHit = Physics.SphereCast(
            center, radius, Vector3.down, out _,
            halfH + groundCheckDistance, mask, QueryTriggerInteraction.Ignore);

        bool ceilingHit = Physics.SphereCast(
            center, radius, Vector3.up, out _,
            halfH + ceilingCheckDistance, mask, QueryTriggerInteraction.Ignore);

        if (ceilingHit)
            ctx.frameVelocity.y = Mathf.Min(0f, ctx.frameVelocity.y);

        ctx.grounded = groundHit;
        ctx.ceilingHit = ceilingHit;
    }
}