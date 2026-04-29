using UnityEngine;

public class HealingBehavior : ItemBehavior<HealingItem>
{
    protected override void OnUseStart() => Heal();

    private void Heal()
    {
        if (Player.TryGetComponent<HealthComponent>(out var comp))
        {
            if (comp.Heal(Item.health) > 0)
            {
                OnItemUsedUp();
            }
        }
    }
}