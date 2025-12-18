using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public Vector3 spawnPosition;
    // On peut garder maxLifeTime au cas où, mais il ne sera plus géré ici par Update

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        // Reset physique à chaque réutilisation via le Pool
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // --- ON SUPPRIME L'UPDATE (Objectif Task 2) ---
    // Le nettoyage est maintenant fait par ProjectilePool.Instance.CleanupRoutine()

    public void Launch(Vector3 direction, float speed)
    {
        // On enregistre la position de départ au moment du tir
        spawnPosition = transform.position;
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // On retourne au pool immédiatement en cas de choc
        ProjectilePool.Instance.ReturnProjectile(gameObject);
    }
}