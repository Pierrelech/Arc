using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Durée de vie (plus utilisée via timer, mais tu peux la garder pour info)")]
    public float maxLifeTime = 5f;   // optionnel maintenant, on passe à la distance

    private Rigidbody rb;
    private TrailRenderer trail;

    // 🔹 Position de départ du projectile (utilisée par le gestionnaire pour la distance)
    [HideInInspector] public Vector3 spawnPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        // Reset physique
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset visuel (trail)
        if (trail != null)
        {
            trail.Clear();
        }

        // On mémorise la position de "spawn" pour le calcul de distance
        spawnPosition = transform.position;
    }
    public void Launch(Vector3 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ici tu peux gérer dégâts / FX / etc.
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (ProjectilePool.Instance != null)
        {
            ProjectilePool.Instance.ReturnProjectile(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
