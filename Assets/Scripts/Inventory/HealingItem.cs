using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/HealingItem")]
public class HealingItem : ConsumableItem, IStackable
{
    public float health = 10f;

    [SerializeField] private int stackSize;
    public int MaxStackSize => stackSize;

    public override bool Use(PlayerStateDriver3D player)
    {
        if (player.TryGetComponent<HealthComponent>(out var comp))
        {
            if (comp.Heal(health) > 0)
            {
                return true;
            }
        }
        return false;
    }
}
