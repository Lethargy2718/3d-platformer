using UnityEngine;

public class PlayerMove : State
{
    readonly PlayerContext ctx;

    public PlayerMove(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if (ctx.moveInput.sqrMagnitude <= 0.01f)
        {
            return ((PlayerGrounded)Parent).PlayerIdle;
        }

        return null;
    }

    protected override void OnEnter()
    {
        ctx.targetBobStrength = ctx.bobMoveStrength;
    }

    protected override void OnFixedUpdate(float fixedDeltaTime)
    {
        ctx.frameVelocity = MovementUtils.ApplyHorizontal(ctx.frameVelocity, ctx.moveDirection, ctx.CurrentMaxSpeed, ctx.acceleration, ctx.groundDecel, fixedDeltaTime);
    }

    protected override void OnExit()
    {
        ctx.targetBobStrength = 0.0f;
    }
}