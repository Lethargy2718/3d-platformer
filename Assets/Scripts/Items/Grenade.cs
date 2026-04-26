using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float fuseDuration = 2f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private float dmg = 10f;
    [SerializeField] private ParticleSystem ps;

    private void Start()
    {
        StartCoroutine(ExplosionCoroutine());
    }

    private IEnumerator ExplosionCoroutine()
    {
        yield return new WaitForSeconds(fuseDuration);
        Explode();
    }

    private void Explode()
    {
        Instantiate(ps, transform.position, ps.transform.rotation);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            if (hit.TryGetComponent<HealthComponent>(out var hc))
            {
                Vector3 direction = (hit.transform.position - transform.position).normalized;
                hc.TakeDamage(dmg, direction);
            }
        }

        Destroy(gameObject);
    }
}
