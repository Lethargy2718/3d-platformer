using UnityEngine;

public class InventorySlot
{
    public Item item;
    public int count;
    public ItemBehavior behavior;

    public InventorySlot(Item item, int count)
    {
        this.item = item;
        this.count = count;
        behavior = Object.Instantiate(item.itemBehavior);
    }

    public override string ToString()
    {
        return $"{item}: {count}";
    }
}