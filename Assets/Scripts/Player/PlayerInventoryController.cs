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
    private Item CurrentItem => Inventory.Slots[currentSlotIndex]?.item;
    private ItemBehavior currentBehavior;
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
        inputHandler.Scrolled += HandleScroll;
        inputHandler.UsePressed += () => currentBehavior?.UseStart();
        inputHandler.UseCanceled += () => currentBehavior?.UseEnd();
        Inventory.SlotChanged += OnInventorySlotChanged;
        UpdateCurrentItem(0);
    }

    private void OnInventorySlotChanged(int idx)
    {
        if (idx == currentSlotIndex) UpdateCurrentItem(currentSlotIndex);
    }

    private void HandleScroll(int delta)
    {
        int count = Inventory.Slots.Count;
        int newIndex = (currentSlotIndex + delta + count) % count;
        if (newIndex == currentSlotIndex) return;
        UpdateCurrentItem(newIndex);
    }

    private void HoldVisuals(ref GameObject currentObject, GameObject prefab, Transform holder)
    {
        Destroy(currentObject);
        currentObject = prefab != null ? Instantiate(prefab, holder) : null;
    }

    private void UpdateHoldVisuals(Item item)
    {
        HoldVisuals(ref currentViewObject, item != null ? item.viewPrefab : null, viewItemHolder);
        HoldVisuals(ref currentWorldObject, item != null ? item.worldPrefab : null, worldItemHolder);
    }

    private void UpdateBehavior(Item item)
    {
        if (currentBehavior) currentBehavior.OnUnequip();

        Destroy(currentBehavior);
        currentBehavior = null;

        if (item == null || item.itemBehavior == null) return;

        currentBehavior = Instantiate(item.itemBehavior, transform).GetComponent<ItemBehavior>();
        currentBehavior.Init(player, item);
        currentBehavior.OnEquip();
    }

    private void UpdateCurrentItem(int newIdx)
    {
        currentSlotIndex = newIdx;
        toolbarUI.SetHighlight(newIdx);

        var item = CurrentItem;
        UpdateHoldVisuals(item);
        UpdateBehavior(item);
    }
}