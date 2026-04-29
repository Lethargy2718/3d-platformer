using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private Transform viewItemHolder;
    [SerializeField] private Transform worldItemHolder;
    [SerializeField] private ToolbarUI toolbarUI;
    [SerializeField] private int inventorySize = 8;

    public Inventory Inventory { get; private set; }

    private int currentSlotIdx;
    private Item CurrentItem => Inventory[currentSlotIdx]?.item;
    private ItemBehavior CurrentBehavior  => Inventory[currentSlotIdx]?.behavior;

    private PlayerController player;
    public GameObject currentViewObject;
    public GameObject currentWorldObject;

    private void Awake()
    {
        Inventory = new Inventory(inventorySize);
    }

    public void Init(PlayerController player, PlayerInputHandler inputHandler)
    {
        this.player = player;
        toolbarUI.Init(Inventory);

        inputHandler.Scrolled += HandleScroll;
        inputHandler.UsePressed += () => CurrentBehavior?.UseStart();
        inputHandler.UseCanceled += () => CurrentBehavior?.UseEnd();
        Inventory.SlotChanged += OnInventorySlotChanged;

        currentSlotIdx = 0;
        RefreshCurrentSlot();
    }

    private void OnInventorySlotChanged(int idx, SlotChangeType type)
    {
        if (type == SlotChangeType.Item)
        {
            if (idx == currentSlotIdx) 
                RefreshCurrentSlot();
            else
            {
                var slot = Inventory[idx];
                if (slot != null && slot.behavior != null) slot.behavior.Init(player, slot.item);
            }
        }
    }

    private void HandleScroll(int delta)
    {
        int count = Inventory.Slots.Count;
        int newIndex = (currentSlotIdx + delta + count) % count;
        if (newIndex == currentSlotIdx) return;
        SwitchSlot(newIndex);
        toolbarUI.SetHighlight(newIndex);
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

    private void SwitchSlot(int idx)
    {
        CurrentBehavior?.OnUnequip();
        currentSlotIdx = idx;
        CurrentBehavior?.OnEquip();
        UpdateHoldVisuals(CurrentItem);
    }

    private void RefreshCurrentSlot()
    {
        CurrentBehavior?.OnUnequip();
        CurrentBehavior?.Init(player, CurrentItem);
        CurrentBehavior?.OnEquip();
        UpdateHoldVisuals(CurrentItem);
    }

    private void Update()
    {
        if (CurrentBehavior != null) CurrentBehavior.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (CurrentBehavior != null) CurrentBehavior.FixedTick(Time.deltaTime);
    }
}
