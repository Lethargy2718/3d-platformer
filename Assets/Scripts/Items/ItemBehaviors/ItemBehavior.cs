using System;
using UnityEngine;

public abstract class ItemBehavior : MonoBehaviour
{
    public event Action ItemUsedUp;

    protected PlayerController Player { get; set; }

    private bool useHeld = false;

    public abstract void Init(PlayerController player, Item item);

    // Lifecycle hooks
    public virtual void OnEquip() { }
    protected virtual void OnUseStart() { }
    protected virtual void OnUpdateHold(float dt) { }
    protected virtual void OnFixedUpdateHold(float dt) { }
    protected virtual void OnUseEnd() { }
    protected virtual void OnTick(float dt) { }
    protected virtual void OnFixedTick(float dt) { }
    public virtual void OnUnequip() { }

    public void UseStart()
    {
        useHeld = true;
        OnUseStart();
    }

    public void UseEnd()
    {
        useHeld = false;
        OnUseEnd();
    }

    public void Tick(float dt)
    {
        if (useHeld) OnUpdateHold(dt);
        OnTick(dt);
    }

    public void FixedTick(float dt)
    {
        if (useHeld) OnFixedUpdateHold(dt);
        OnFixedTick(dt);
    }

    protected void OnItemUsedUp() => ItemUsedUp?.Invoke();
}

public abstract class ItemBehavior<T> : ItemBehavior where T : Item
{
    protected T Item { get; set; }

    public override void Init(PlayerController player, Item item)
    {
        Player = player;

        if (item is not T itemT)
            throw new InvalidOperationException($"{GetType().Name} expected {typeof(T).Name} but got {item.GetType().Name}");

        Item = itemT;
    }
}