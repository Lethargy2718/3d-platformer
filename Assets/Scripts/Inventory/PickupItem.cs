using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerStateDriver3D>(out var player))
        {
            player.inventory.AddItem(item, 1);
        }
    }
}
