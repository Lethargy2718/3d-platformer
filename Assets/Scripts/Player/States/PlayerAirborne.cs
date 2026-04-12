using UnityEngine;

public class PlayerAirborne : State
{
    readonly PlayerContext ctx;
    public readonly PlayerJump PlayerJump;
    public readonly PlayerFall PlayerFall;

    public PlayerAirborne(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        PlayerJump = new PlayerJump(m, this, ctx);
        PlayerFall = new PlayerFall(m, this, ctx);
    }

    protected override State GetInitialState() => PlayerFall;

    protected override void OnFixedUpdate(float fixedDeltaTime)
    {
        HandleGravity(fixedDeltaTime);

        ctx.frameVelocity = MovementUtils.ApplyHorizontal(ctx.frameVelocity, ctx.moveDirection, ctx.CurrentMaxSpeed, ctx.airAcceleration, ctx.airFriction, fixedDeltaTime);
    }

    protected override void OnCollision(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y < ctx.maxWallNormalY)
            {
                ctx.frameVelocity.x = 0;
                ctx.frameVelocity.z = 0;
            }
        }
    }

    private void HandleGravity(float dt)
    {
        float gravityThisFrame = ctx.fallAcceleration * dt;

        if (ctx.endedJumpEarly && ctx.frameVelocity.y > 0f)
        {
            gravityThisFrame *= ctx.jumpEndEarlyGravityModifier;
        }

        ctx.frameVelocity.y -= gravityThisFrame;
        ctx.frameVelocity.y = Mathf.Max(ctx.frameVelocity.y, -ctx.maxFallSpeed);
    }
}