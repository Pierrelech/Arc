using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    [Header("Pool de projectiles")]
    public GameObject projectilePrefab;   // Mets ici ton prefab de balle (le même que dans ton Gun avant)
    public int initialSize = 20;          // Nombre de projectiles pré-instanciés
    [Header("Nettoyage par distance")]
    public float maxDistance = 30f;       // distance max depuis la position de spawn
    public float cleanupInterval = 0.25f; // entre 0.2 et 0.5 s comme demandé
    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activeProjectiles = new List<GameObject>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject proj = Instantiate(projectilePrefab, transform);
            proj.SetActive(false);
            pool.Enqueue(proj);
        }
        StartCoroutine(CleanupRoutine());
    }

    public GameObject GetProjectile()
    {
        GameObject proj;

        if (pool.Count > 0)
            proj = pool.Dequeue();
        else
            proj = Instantiate(projectilePrefab, transform);

        proj.SetActive(true); // ✅ OBLIGATOIRE

        // On l'ajoute à la liste des actifs
        activeProjectiles.Add(proj);

        return proj;
    }


    public void ReturnProjectile(GameObject proj)
    {
        activeProjectiles.Remove(proj);
        proj.SetActive(false);
        pool.Enqueue(proj);
    }

    private IEnumerator CleanupRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(cleanupInterval);

        while (true)
        {
            yield return wait;

            float maxSqr = maxDistance * maxDistance;

            // On parcourt à l'envers pour pouvoir Remove sans souci
            for (int i = activeProjectiles.Count - 1; i >= 0; i--)
            {
                GameObject proj = activeProjectiles[i];

                // Sécurité si l’objet a été détruit ou désactivé
                if (proj == null || !proj.activeSelf)
                {
                    activeProjectiles.RemoveAt(i);
                    continue;
                }

                Projectile p = proj.GetComponent<Projectile>();
                if (p == null)
                {
                    activeProjectiles.RemoveAt(i);
                    continue;
                }

                Vector3 offset = proj.transform.position - p.spawnPosition;
                float sqrDist = offset.sqrMagnitude;

                if (sqrDist > maxSqr)
                {
                    // Trop loin → on le renvoie au pool
                    ReturnProjectile(proj);
                }
            }
        }
    }
}
