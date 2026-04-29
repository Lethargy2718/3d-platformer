using UnityEngine;

[CreateAssetMenu(menuName = "Items/Gun")]
public class GunItem : Item
{
    public ParticleSystem muzzleParticles;
    public ParticleSystem impactParticles;
    public float fireRate; // Shots per second
    public float bulletDamage;
    public float maxDistance;
    public int maxAmmo;
    public float reloadDuration;
}