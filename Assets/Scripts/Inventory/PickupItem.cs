using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item;

    public void GiveItemToPlayer(PlayerController player)
    {
        // If given successfully
        if (player.Inventory.AddItem(item, 1) == 0) 
        {
            Destroy(gameObject);
        } 
    }
}
