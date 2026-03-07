using UnityEngine;

public class PlayerIdle : State
{
    readonly PlayerContext ctx;

    public PlayerIdle(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if (ctx.moveInput.sqrMagnitude > 0.01f)
        {
            return ((PlayerGrounded)Parent).PlayerMove;
        }

        return null;
    }

    protected override void OnEnter()
    {
        ctx.frameVelocity.x = 0f;
        ctx.frameVelocity.z = 0f;
    }
}