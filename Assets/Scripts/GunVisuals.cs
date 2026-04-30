using UnityEngine;

public class GunVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem muzzleParticles;
    [SerializeField] private Transform muzzleTransform;

    public ParticleSystem MuzzleParticles => muzzleParticles;
    public Transform MuzzleTransform => muzzleTransform;
}
