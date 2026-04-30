using UnityEngine;

public class InventorySlot
{
    public Item item;
    public int count;
    public ItemBehavior behavior;

    public InventorySlot(Item item, int count, ItemBehavior existingBehavior = null)
    {
        this.item = item;
        this.count = count;
        this.behavior = existingBehavior == null ? Object.Instantiate(item.itemBehavior) : existingBehavior; 
    }

    public override string ToString()
    {
        return $"{item}: {count}";
    }
}