using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private Transform viewItemHolder;
    [SerializeField] private Transform worldItemHolder;
    [SerializeField] private ToolbarUI toolbarUI;
    [SerializeField] private int inventorySize = 8;

    public Inventory Inventory { get; private set; }

    private int currentSlotIndex;
    private PlayerController player;

    private InventorySlot CurrentSlot => Inventory[currentSlotIndex];
    private Item CurrentItem => CurrentSlot?.item;
    private GameObject currentViewObject;
    private GameObject currentWorldObject;


    private void Awake()
    {
        Inventory = new Inventory(inventorySize);
    }

    public void Init(PlayerController player, PlayerInputHandler inputHandler)
    {
        this.player = player;

        toolbarUI.Init(Inventory);
        UpdateHold();

        inputHandler.OnScrolled += HandleScroll;
        inputHandler.OnUsePressed += HandleUse;
        Inventory.SlotChanged += OnInventorySlotChanged;
    }

    private void OnInventorySlotChanged(int idx)
    {
        if (idx == currentSlotIndex) UpdateHold();
    }

    private void HandleScroll(int delta)
    {
        int count = Inventory.Slots.Count;
        int newIndex = (currentSlotIndex + delta + count) % count;

        if (newIndex == currentSlotIndex) return;

        currentSlotIndex = newIndex;
        toolbarUI.SetHighlight(currentSlotIndex);
        UpdateHold();
    }

    private void HandleUse()
    {
        if (CurrentItem == null) return;

        if (CurrentItem.Use(player))
            Inventory.RemoveItem(currentSlotIndex, 1);
    }

    private void Hold(ref GameObject currentObject, GameObject heldPrefab, Transform holder)
    {
        Destroy(currentObject);
        if (heldPrefab == null) return;
        currentObject = Instantiate(heldPrefab, holder);
    }

    private void UpdateHold()
    {
        var item = Inventory.Slots[currentSlotIndex]?.item;

        Hold(ref currentViewObject, item != null ? item.viewPrefab : null, viewItemHolder);
        Hold(ref currentWorldObject, item != null ? item.worldPrefab : null, worldItemHolder);
    }
}