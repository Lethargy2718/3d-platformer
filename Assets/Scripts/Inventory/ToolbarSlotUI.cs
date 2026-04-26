using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolbarSlotUI : MonoBehaviour
{
    [SerializeField] private Image border;
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;

    public void Refresh(InventorySlot slot)
    {
        bool empty = slot == null;

        if (empty)
        {
            icon.enabled = false;
            countText.text = "";
        }
        else
        {
            icon.enabled = true;
            icon.sprite = slot.item.itemIcon;
            countText.text = slot.item is IStackable ? slot.count.ToString() : "";
        }
    }

    public void SetHighlight(bool highlighted)
    {
        border.gameObject.SetActive(highlighted);
    }
}