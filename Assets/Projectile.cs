using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Dur�e de vie")]
    public float maxLifeTime = 5f;   // temps avant retour au pool si rien touch�

    private Rigidbody rb;
    private float lifeTimer;

    private TrailRenderer trail;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        // Reset timer
        lifeTimer = 0f;

        // Reset physique
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset visuel (trail)
        if (trail != null)
        {
            trail.Clear();
        }
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLifeTime)
        {
            ReturnToPool();
        }
    }

    /// <summary>
    /// Lancement du projectile dans une direction avec une certaine vitesse.
    /// </summary>
    public void Launch(Vector3 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ici tu peux g�rer d�g�ts / FX / etc.

        ReturnToPool();
    }

    private void ReturnToPool()
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
