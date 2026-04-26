using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out var player))
        {
            player.Inventory.AddItem(item, 1);
        }
    }
}
