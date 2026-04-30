using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private Transform viewItemHolder;
    [SerializeField] private Transform worldItemHolder;
    [SerializeField] private ToolbarUI toolbarUI;
    [SerializeField] private int inventorySize = 8;

    public Inventory Inventory { get; private set; }

    private int currentSlotIdx;

    private Item equippedItem;
    private ItemBehavior equippedBehavior;

    [HideInInspector] public GameObject currentViewObject;
    [HideInInspector] public GameObject currentWorldObject;
     
    private PlayerController player;
    private PlayerInputHandler inputHandler;

    private void Awake()
    {
        Inventory = new Inventory(inventorySize);
    }

    public void Init(PlayerController player, PlayerInputHandler inputHandler)
    {
        this.player = player;
        this.inputHandler = inputHandler;
        toolbarUI.Init(Inventory);

        this.inputHandler.Scrolled += HandleScroll;
        this.inputHandler.UsePressed += () =>
        {
            if (equippedBehavior != null) equippedBehavior.UseStart();
        };
        this.inputHandler.UseCanceled += () =>
        {
            if (equippedBehavior != null) equippedBehavior.UseEnd();
        };
        Inventory.SlotChanged += OnInventorySlotChanged;

        currentSlotIdx = 0;
        RefreshCurrentSlot();
    }

    private void OnInventorySlotChanged(int idx, SlotChangeType type)
    {
        if (type != SlotChangeType.Item) return;

        var slot = Inventory[idx];

        if (idx == currentSlotIdx)
        {
            UnequipCurrent();
            EquipFromSlot(currentSlotIdx);
        }
        else
        {
            if (slot?.behavior != null) slot.behavior.Init(player, slot.item);
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

    private void SwitchSlot(int newSlotIdx)
    {
        UnequipCurrent();
        currentSlotIdx = newSlotIdx;
        EquipFromSlot(currentSlotIdx);
    }

    private void RefreshCurrentSlot()
    {
        UnequipCurrent();
        EquipFromSlot(currentSlotIdx);
    }

    private void EquipFromSlot(int slotIdx)
    {
        var slot = Inventory[slotIdx];
        equippedItem = slot?.item;
        equippedBehavior = slot?.behavior;
        UpdateHoldVisuals(equippedItem);

        if (equippedBehavior != null)
        {
            equippedBehavior.Init(player, equippedItem);
            equippedBehavior.ItemUsedUp += OnItemUsedUp;
            equippedBehavior.OnEquip();
        }

    }

    private void UnequipCurrent()
    {
        if (equippedBehavior != null)
        {
            equippedBehavior.OnUnequip();
            equippedBehavior.ItemUsedUp -= OnItemUsedUp;
            equippedBehavior = null;
        }

        equippedItem = null;

        if (currentViewObject != null) Destroy(currentViewObject);
        if (currentWorldObject != null) Destroy(currentWorldObject);
        currentViewObject = null;
        currentWorldObject = null;
    }

    private void UpdateHoldVisuals(Item item)
    {
        if (item == null) return;

        currentViewObject = item.viewPrefab != null ? Instantiate(item.viewPrefab, viewItemHolder) : null;
        currentWorldObject = item.worldPrefab != null ? Instantiate(item.worldPrefab, worldItemHolder) : null;
    }

    private void OnItemUsedUp()
    {
        Inventory.RemoveItem(currentSlotIdx, 1);
    }

    private void Update()
    {
        if (equippedBehavior != null) equippedBehavior.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (equippedBehavior != null) equippedBehavior.FixedTick(Time.fixedDeltaTime);
    }
}