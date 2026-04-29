using UnityEngine;

public class ThrowableBehavior : ItemBehavior<ThrowableItem>
{
    // TODO: find a way to pass this in somehow while keeping it constant across all throwable items?? maybe pass from player?
    private const float throwForce = 15f; 

    protected override void OnUseStart()
    {
        Throw();
        OnItemUsedUp();
    }

    private void Throw()
    {
        Vector3 aimDir = Player.GetAimDirection(Player.AimOrigin.position);
        GameObject thrown = Instantiate(Item.throwablePrefab, Player.AimOrigin.position, Quaternion.LookRotation(aimDir));
        if (thrown.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = aimDir * throwForce;
        }
    }
}