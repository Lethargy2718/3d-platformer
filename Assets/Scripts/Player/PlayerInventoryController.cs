using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private Transform itemHolder;
    [SerializeField] private ToolbarUI toolbarUI;
    [SerializeField] private int inventorySize = 8;

    public Inventory Inventory { get; private set; }

    private int currentSlotIndex;
    private PlayerController player;

    private InventorySlot CurrentSlot => Inventory[currentSlotIndex];
    private Item CurrentItem => CurrentSlot?.item;
    private GameObject currentHeldObject;

    private void Start()
    {
        Inventory = new Inventory(inventorySize);
    }

    public void Init(PlayerController player, PlayerInputHandler inputHandler)
    {
        this.player = player;

        toolbarUI.Init(Inventory);
        Hold(Inventory[currentSlotIndex]?.item?.heldPrefab);

        inputHandler.OnScrolled += HandleScroll;
        inputHandler.OnUsePressed += HandleUse;
        Inventory.SlotChanged += OnInventorySlotChanged;
    }

    private void OnInventorySlotChanged(int idx)
    {
        if (idx == currentSlotIndex && Inventory[idx] == null) Hold(null);
    }

    private void HandleScroll(int delta)
    {
        int count = Inventory.Slots.Count;
        int newIndex = (currentSlotIndex + delta + count) % count;

        if (newIndex == currentSlotIndex) return;

        currentSlotIndex = newIndex;
        toolbarUI.SetHighlight(currentSlotIndex);
        Hold(Inventory[currentSlotIndex]?.item?.heldPrefab);
    }

    private void HandleUse()
    {
        if (CurrentItem == null) return;

        if (CurrentItem.Use(player))
            Inventory.RemoveItem(currentSlotIndex, 1);
    }

    private void Hold(GameObject heldPrefab)
    {
        Destroy(currentHeldObject);
        if (heldPrefab == null) return;
        currentHeldObject = Instantiate(heldPrefab, itemHolder);
    }
}