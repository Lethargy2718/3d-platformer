using UnityEngine;

public class PlayerDash : State
{
    readonly PlayerContext ctx;
    float timer;
    bool endDash;

    public PlayerDash(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override void OnEnter()
    {
        endDash = false;
        timer = 0f;

        Vector3 dir = ctx.dashDirection.sqrMagnitude > 0.01f ? ctx.dashDirection : ctx.transform.forward;

        ctx.frameVelocity = dir.normalized * ctx.dashSpeed;
        ctx.hasBufferedDash = false;
        if (!ctx.grounded) ctx.hasAirDashed = true;
        ctx.InvokeDashed();
    }

    protected override State GetTransition()
    {
        if (endDash)
        {
            if (ctx.grounded)
            {
                return ((PlayerRoot)Parent).PlayerGrounded;
            }

            return ((PlayerRoot)Parent).PlayerAirborne;
        }

        if (ctx.jumpPressed)
        {
            return ((PlayerRoot)Parent).PlayerAirborne.PlayerJump;
        }

        return null;
    }

    protected override void OnUpdate(float deltaTime)
    {
        timer += deltaTime;
        if (timer >= ctx.dashDuration)
            endDash = true;
    }

    protected override void OnExit()
    {
        ctx.timeDashEnded = ctx.time;
        ctx.InvokeDashEnded();
    }
}