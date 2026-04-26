using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private ToolbarUI toolbarUI;

    public Inventory Inventory { get; } = new(8);

    private int currentSlotIndex;
    private PlayerController player;

    private InventorySlot CurrentSlot => Inventory.Slots[currentSlotIndex];
    private Item CurrentItem => CurrentSlot?.item;

    public void Init(PlayerController player, PlayerInputHandler inputHandler)
    {
        this.player = player;

        toolbarUI.Init(Inventory);

        inputHandler.OnScrolled += HandleScroll;
        inputHandler.OnUsePressed += HandleUse;
    }

    private void HandleScroll(int delta)
    {
        int count = Inventory.Slots.Count;
        int newIndex = (currentSlotIndex + delta + count) % count;

        if (newIndex == currentSlotIndex) return;

        currentSlotIndex = newIndex;
        toolbarUI.SetHighlight(currentSlotIndex);
    }

    private void HandleUse()
    {
        if (CurrentItem == null) return;

        if (CurrentItem.Use(player))
            Inventory.RemoveItem(currentSlotIndex, 1);
    }
}