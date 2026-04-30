using TMPro;
using UnityEngine;

public class GunUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    private GunBehavior currentGun;
    private bool reloading = false;

    private void OnEnable()
    {
        GunBehavior.GunEquipped += OnGunEquipped;
        GunBehavior.GunUnEquipped += OnGunUnEquipped;
        GunBehavior.GunStartedReloading += OnGunStartedReloading;
        GunBehavior.GunFinishedReloading += OnGunFinishedReloading;
    }

    private void OnDisable()
    {
        GunBehavior.GunEquipped -= OnGunEquipped;
        GunBehavior.GunUnEquipped -= OnGunUnEquipped;
        GunBehavior.GunStartedReloading -= OnGunStartedReloading;
        GunBehavior.GunFinishedReloading -= OnGunFinishedReloading;
    }

    private void OnGunEquipped(GunBehavior gun)
    {
        ammoText.enabled = true;
        currentGun = gun;
    }

    private void OnGunUnEquipped(GunBehavior gun)
    {
        ammoText.enabled = false;
        currentGun = null;
        reloading = false;
    }

    private void OnGunStartedReloading()
    {
        ammoText.text = "Reloading...";
        reloading = true;
    }

    private void OnGunFinishedReloading()
    {
        reloading = false;
    }

    private void Update()
    {
        if (currentGun != null && !reloading)
            ammoText.text = $"{currentGun.CurrentAmmo} / {currentGun.Item.maxAmmo}";
    }
}