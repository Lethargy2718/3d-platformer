using System.Collections;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class GunBehavior : ItemBehavior<GunItem>
{
    // TODO: subscribe in UI to display/hide ammo
    public static event Action<GunBehavior> GunEquipped;
    public static event Action<GunBehavior> GunUnEquipped;
    public static event Action GunStartedReloading;
    public static event Action GunFinishedReloading;

    public int CurrentAmmo { get; private set; }
    private float timeTillNextShot = 0f;
    private bool reloading = false;

    public override void Init(PlayerController player, Item item)
    {
        base.Init(player, item);
        CurrentAmmo = Item.maxAmmo;
    }

    public override void OnEquip()
    {
        GunEquipped?.Invoke(this);
        //if (CurrentAmmo <= 0) StartCoroutine(ReloadCoroutine());
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
        // TODO: connect to an event somehow
        if (Keyboard.current.rKey.isPressed && CurrentAmmo != Item.maxAmmo)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    private void Shoot()
    {
        if (CurrentAmmo <= 0) return;
        if (reloading)
        {
            EndReload();
            StopAllCoroutines();
        }

        var aimOrigin = Player.AimOrigin;
        var aimDirection = Player.GetAimDirection(aimOrigin.position);

        if (Physics.Raycast(aimOrigin.position, aimDirection, out var hit, Item.maxDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            // TODO: play and stop particles instead of spawning/destroying
            Instantiate(Item.impactParticles, hit.point, Quaternion.LookRotation(hit.normal));
            if (hit.collider.TryGetComponent<IHittable>(out var hittable))
            {
                hittable.GetHit(Item.bulletDamage, (hit.collider.transform.position - hit.point).normalized);
            }
            Debug.Log(hit.collider.gameObject.name);
        }

        var muzzleTransform = Player.InventoryController.currentViewObject.TryGetComponent<TransformExposer>(out var exposer) ? exposer.T : aimOrigin;

        // TODO: play and stop particles instead of spawning/destroying
        var ps = Instantiate(Item.muzzleParticles, Player.InventoryController.currentViewObject.transform);
        ps.transform.SetPositionAndRotation(muzzleTransform.position, muzzleTransform.rotation);

        // NOTE: if i ever add a beam or whatever later, i can spawn from muzzleTransform.position toward hit.point while keeping the actual origin = player's aim origin

        if (--CurrentAmmo <= 0)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {

        StartReload();
        yield return new WaitForSeconds(Item.reloadDuration);
        EndReload();
        CurrentAmmo = Item.maxAmmo;
    }

    private void StartReload()
    {
        reloading = true;
        GunStartedReloading?.Invoke();
    }

    private void EndReload()
    {
        reloading = false;
        GunFinishedReloading?.Invoke();
    }

    private void OnDestroy()
    {

    }
}