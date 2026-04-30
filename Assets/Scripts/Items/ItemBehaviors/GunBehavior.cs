using System.Collections;
using UnityEngine;
using System;

public class GunBehavior : ItemBehavior<GunItem>
{
    public static event Action<GunBehavior> GunEquipped;
    public static event Action<GunBehavior> GunUnEquipped;
    public static event Action GunStartedReloading;
    public static event Action GunFinishedReloading;

    public int CurrentAmmo { get; private set; }
    private float timeTillNextShot = 0f;
    private bool isReloading = false;

    private GunVisuals gunVisualsView;
    private GunVisuals gunVisualsWorld;

    private ParticleSystem muzzleParticlesView;
    private ParticleSystem muzzleParticlesWorld;

    public override void Init(PlayerController player, Item item)
    {
        base.Init(player, item);
        if (!isInitialized)
        {
            CurrentAmmo = Item.maxAmmo;
            isInitialized = true;
        }
    }

    public override void OnEquip()
    {
        GunEquipped?.Invoke(this);

        // NOTE: assumes currentViewObject and currentWorldObject were set before calling Init
        if (!Player.InventoryController.currentViewObject.TryGetComponent(out gunVisualsView))
        {
            Debug.LogError("No GunVisuals on object gunVisualView");
            Debug.Log(Player.InventoryController.currentViewObject);
        }
        else muzzleParticlesView = gunVisualsView.MuzzleParticles;

        if (!Player.InventoryController.currentWorldObject.TryGetComponent(out gunVisualsWorld))
        {
            Debug.LogError("No GunVisuals on object gunWorldView");
            Debug.Log(Player.InventoryController.currentWorldObject);
        }
        else muzzleParticlesWorld = gunVisualsWorld.MuzzleParticles;

        Player.InputHandler.ReloadPressed += Reload;

        if (CurrentAmmo <= 0) Reload();
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        GunUnEquipped?.Invoke(this);
        muzzleParticlesView.Stop();
        muzzleParticlesWorld.Stop();
        StopAllCoroutines();
        isReloading = false;
        Player.InputHandler.ReloadPressed -= Reload;
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

    private void Shoot()
    {
        if (CurrentAmmo <= 0)
        {
            if (!isReloading)
            {
                Reload();
            }
            return;
        }
        if (isReloading)
        {
            EndReload();
            StopAllCoroutines();
        }

        var aimOrigin = Player.AimOrigin;
        var aimDirection = Player.GetAimDirection(aimOrigin.position);

        if (Physics.Raycast(aimOrigin.position, aimDirection, out var hit, Item.maxDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            Instantiate(Item.impactParticles, hit.point, Quaternion.LookRotation(hit.normal));
            if (hit.collider.TryGetComponent<IHittable>(out var hittable))
            {
                hittable.GetHit(Item.bulletDamage, (hit.collider.transform.position - hit.point).normalized);
            }
        }

        muzzleParticlesView.Play();
        muzzleParticlesWorld.Play();

        // NOTE: if i ever add a beam or whatever later, i can spawn from muzzleTransform.position toward hit.point while keeping the actual origin = player's aim origin

        if (--CurrentAmmo <= 0)
        {
            Reload();
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
        isReloading = true;
        GunStartedReloading?.Invoke();
    }

    private void EndReload()
    {
        isReloading = false;
        GunFinishedReloading?.Invoke();
    }

    private void Reload()
    {
        if (isReloading || CurrentAmmo == Item.maxAmmo) return;
        StartCoroutine(ReloadCoroutine());
    }
}