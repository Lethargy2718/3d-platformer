using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ContentSizeFitter), typeof(HorizontalLayoutGroup))]
public class ToolbarUI : MonoBehaviour
{
    [SerializeField] private ToolbarSlotUI slotPrefab;

    private Inventory inventory;
    private ToolbarSlotUI[] slotUIs;
    private int currentIdx = 0;

    public void Init(Inventory inv)
    {
        inventory = inv;
        slotUIs = new ToolbarSlotUI[inventory.Slots.Count];

        for (int i = 0; i < inventory.Slots.Count; i++)
        {
            var ui = Instantiate(slotPrefab, transform);
            ui.Refresh(inventory[i]);
            slotUIs[i] = ui;
        }

        inventory.SlotChanged += (idx, _) => slotUIs[idx].Refresh(inventory[idx]);
        SetHighlight(currentIdx);
    }

    public void SetHighlight(int idx)
    {
        slotUIs[currentIdx].SetHighlight(false);
        currentIdx = idx;
        slotUIs[currentIdx].SetHighlight(true);

    }
}