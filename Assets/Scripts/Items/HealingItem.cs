using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/HealingItem")]
public class HealingItem : ConsumableItem, IStackable
{
    public float health = 10f;

    [SerializeField] private int stackSize;
    public int MaxStackSize => stackSize;
}
