[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int count;

    public InventorySlot(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }
}