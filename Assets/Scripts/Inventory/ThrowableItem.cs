using UnityEngine;

[CreateAssetMenu(menuName = "Items/Throwable")]
public class ThrowableItem : Item, IStackable
{
    public GameObject throwableItemPrefab;

    public override bool Use(PlayerController player)
    {
        return player.Throw(this);
    }
}
