using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private Transform viewItemHolder;
    [SerializeField] private Transform worldItemHolder;
    [SerializeField] private ToolbarUI toolbarUI;
    [SerializeField] private int inventorySize = 8;

    [Header("Drop Settings")]
    [SerializeField] private float dropForwardDistance = 1.5f;
    [SerializeField] private float dropUpOffset = 0.5f;
    [SerializeField] private float singleDropRandomRadius = 0.3f;
    [SerializeField] private float multiDropRadius = 0.5f;
    [SerializeField] private float multiDropAngleJitter = 15f;
    [SerializeField] private float multiDropVerticalJitter = 0.2f;
    [SerializeField] private float cooldownAfterDrop = 2f;

    public Inventory Inventory { get; private set; }

    private int currentSlotIdx;

    private Item equippedItem;
    private ItemBehavior equippedBehavior;

    [HideInInspector] public GameObject currentViewObject;
    [HideInInspector] public GameObject currentWorldObject;

    private PlayerController player;

    private void Awake()
    {
        Inventory = new Inventory(inventorySize);
    }

    public void Init(PlayerController player, PlayerInputHandler inputHandler)
    {
        this.player = player;
        toolbarUI.Init(Inventory);

        inputHandler.Scrolled += HandleScroll;

        inputHandler.UsePressed += () =>
        {
            if (equippedBehavior != null) equippedBehavior.UseStart();
        };
        inputHandler.UseCanceled += () =>
        {
            if (equippedBehavior != null) equippedBehavior.UseEnd();
        };

        inputHandler.DropPressed += DropCurrentItem;

        inputHandler.DropAllPressed += DropAllCurrentItem;

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

    private void DropItems(int count)
    {
        if (equippedItem == null || count <= 0) return;

        bool isStackable = equippedItem is IStackable;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = GetDropPosition(i, count);
            var drop = Instantiate(equippedItem.pickupPrefab, pos, Quaternion.identity);
            var trigger = drop.GetComponentInChildren<PickupTrigger>();
            if (trigger != null) trigger.StartCooldown(cooldownAfterDrop);

            if (isStackable)
            {
                Inventory.RemoveItem(currentSlotIdx, 1);
                continue;
            }

            // Pass behavior to pickup if not stackable
            var slot = Inventory.TakeSlot(currentSlotIdx);
            var pickup = drop.GetComponentInChildren<PickupItem>();
            if (pickup != null && slot != null)
            {
                pickup.savedBehavior = slot.behavior;
            }
        }
    }
    private void DropCurrentItem() => DropItems(1);
    private void DropAllCurrentItem() => DropItems(Inventory[currentSlotIdx]?.count ?? 0);

    private Vector3 GetDropPosition(int itemIndex, int totalItems)
    {
        Vector3 basePos = player.transform.position + player.transform.forward * dropForwardDistance + Vector3.up * dropUpOffset;

        if (totalItems == 1)
        {
            return basePos + Random.insideUnitSphere * singleDropRandomRadius;
        }

        float angleStep = 360f / totalItems;
        float angle = angleStep * itemIndex + Random.Range(-multiDropAngleJitter, multiDropAngleJitter);

        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * multiDropRadius;

        offset.y = Random.Range(-multiDropVerticalJitter, multiDropVerticalJitter);

        return basePos + offset;
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