using UnityEngine;

[CreateAssetMenu(menuName = "Items/Throwable")]
public class ThrowableItem : Item, IStackable
{
    public override bool Use(PlayerStateDriver3D player)
    {
        return player.Throw(this);
    }
}
