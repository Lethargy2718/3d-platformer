using System.Collections;
using UnityEngine;
using System;

public class GunBehavior : ItemBehavior<GunItem>
{
    // TODO: subscribe in UI to display/hide ammo
    public static event Action<GunBehavior> GunEquipped;
    public static event Action<GunBehavior> GunUnEquipped;

    public int CurrentAmmo { get; private set; }
    private float timeTillNextShot = 0f;

    public override void Init(PlayerController player, Item item)
    {
        base.Init(player, item);
        CurrentAmmo = Item.maxAmmo;
    }

    public override void OnEquip()
    {
        GunEquipped?.Invoke(this);
        if (CurrentAmmo <= 0) StartCoroutine(ReloadCoroutine());
    }

    public override void OnUnequip()
    {
        GunUnEquipped?.Invoke(this);
        StopAllCoroutines();
    }

    protected override void OnUpdateHold(float _)
    {
        if (timeTillNextShot <= 0f)
        {
            timeTillNextShot = 1f / Item.fireRate;
            Shoot();
        }
    }

    protected override void OnTick(float dt)
    {
        timeTillNextShot -= dt;
    }

    // TODO: play and stop particles instead of spawning/destroying
    private void Shoot()
    {
        if (CurrentAmmo <= 0) return;

        Transform muzzleTransform;

        // TODO: come up with a better system than this
        if (Player.InventoryController.currentViewObject.TryGetComponent<TransformExposer>(out var exposer))
        {
            muzzleTransform = exposer.T;
        }
        else
        {
            muzzleTransform = Player.AimOrigin;
        }

        var muzzlePos = muzzleTransform.position;

        var aimDirection = Player.GetAimDirection(muzzlePos);

        if (Physics.Raycast(muzzlePos, aimDirection, out var hit, Item.maxDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            Instantiate(Item.impactParticles, hit.point, Quaternion.LookRotation(hit.normal));
            if (hit.collider.TryGetComponent<IHittable>(out var hittable))
            {
                hittable.GetHit(Item.bulletDamage);
            }
        }

        // TODO: check if there is a one liner
        var ps = Instantiate(Item.muzzleParticles, Player.InventoryController.currentViewObject.transform);
        ps.transform.SetPositionAndRotation(muzzlePos, muzzleTransform.rotation);

        if (--CurrentAmmo <= 0)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(Item.reloadDuration);
        CurrentAmmo = Item.maxAmmo;
    }

    private void OnDestroy()
    {
        
    }
}