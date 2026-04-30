using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public event Action<int, SlotChangeType> SlotChanged; // sends slot idx and change type

    private readonly int size;
    private readonly InventorySlot[] slots;

    public IReadOnlyList<InventorySlot> Slots => slots;
    public InventorySlot this[int idx] => slots[idx];

    public Inventory(int size)
    {
        this.size = size;
        slots = new InventorySlot[size];
    }

    // Returns slot and clears it in the inventory
    public InventorySlot TakeSlot(int idx)
    {
        if (idx < 0 || idx >= size) return null;
        ref var slot = ref slots[idx];
        if (slot == null) return null;
        var taken = slot;
        slot = null;
        SlotChanged?.Invoke(idx, SlotChangeType.Item);
        return taken;
    }

    // Returns the number of leftover items
    public int AddItem(Item item, int count, ItemBehavior existingBehavior = null)
    {
        if (item is IStackable stackableItem)
        {
            return AddItem(stackableItem, count);
        }

        for (int i = 0; i < size && count > 0; i++)
        {
            ref var slot = ref slots[i];
            if (slot == null)
            {
                slot = new InventorySlot(item, 1, count == 1 ? existingBehavior : null);
                count--;
                SlotChanged?.Invoke(i, SlotChangeType.Item);
            }
        }
        return count;
    }

    // Returns the number of leftover items
    public int AddItem(IStackable item, int count)
    {
        // First pass: add to slots that already contain the item
        for (int i = 0; i < size && count > 0; i++)
        {
            ref var slot = ref slots[i];

            if (slot != null && slot.item == (Item)item)
            {
                int spaceLeft = item.MaxStackSize - slot.count;
                int toAdd = Mathf.Min(spaceLeft, count);
                slot.count += toAdd;
                count -= toAdd;

                SlotChanged?.Invoke(i, SlotChangeType.Number);
            }
        }

        // Second pass: add to empty slots
        for (int i = 0; i < size && count > 0; i++)
        {
            ref var slot = ref slots[i];

            if (slot == null)
            {
                slot = new InventorySlot((Item)item, Mathf.Min(item.MaxStackSize, count));
                count -= slot.count;
                SlotChanged?.Invoke(i, SlotChangeType.Item);
            }
        }

        return count;
    }

    public int RemoveItem(int idx, int count)
    {
        if (idx < 0 || idx >= size || count <= 0) return 0;

       ref var slot = ref slots[idx];

        if (slot == null) return 0;

        int toRemove = Mathf.Min(slot.count, count);
        slot.count -= toRemove;

        SlotChangeType type = SlotChangeType.Number;

        if (slot.count <= 0)
        {
            type = SlotChangeType.Item;
            UnityEngine.Object.Destroy(slot.behavior); // Todo: possibly move this somewhere else using the event
            slot = null;
        }
        SlotChanged?.Invoke(idx, type);

        return toRemove;
    }
}

public enum SlotChangeType { Number, Item }