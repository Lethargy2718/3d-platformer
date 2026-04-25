using UnityEngine;

public abstract class ThrowableItem : Item, IStackable
{
    public override bool Use(PlayerStateDriver3D player)
    {
        return player.Throw(this);
    }
}
