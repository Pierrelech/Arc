using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    [Header("Pool Settings")]
    public GameObject projectilePrefab;
    public int initialSize = 20;

    [Header("Task 2 - Centralized Cleanup")]
    public float maxDistance = 30f;       // Distance max de vol
    public float cleanupInterval = 0.2f;  // Entre 0.2 et 0.5s selon consigne

    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activeProjectiles = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
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

        // Lancement du nettoyage centralisé
        StartCoroutine(CleanupRoutine());
    }

    public GameObject GetProjectile()
    {
        GameObject proj;

        if (pool.Count > 0)
            proj = pool.Dequeue();
        else
            proj = Instantiate(projectilePrefab, transform);

        proj.SetActive(true); // <--- CETTE LIGNE EST CRUCIALE
        activeProjectiles.Add(proj);

        return proj;
    }

    public void ReturnProjectile(GameObject proj)
    {
        if (!proj.activeSelf) return; // Évite les doubles retours

        activeProjectiles.Remove(proj);
        proj.SetActive(false);
        pool.Enqueue(proj);
    }

    private IEnumerator CleanupRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(cleanupInterval);
        float maxSqrDist = maxDistance * maxDistance;

        while (true)
        {
            yield return wait;

            // Parcours inverse pour sécurité lors du retrait de la liste
            for (int i = activeProjectiles.Count - 1; i >= 0; i--)
            {
                GameObject proj = activeProjectiles[i];

                if (proj == null || !proj.activeSelf) continue;

                Projectile pScript = proj.GetComponent<Projectile>();
                if (pScript != null)
                {
                    // Calcul de distance par rapport au point de spawn du projectile
                    float sqrDist = (proj.transform.position - pScript.spawnPosition).sqrMagnitude;

                    if (sqrDist > maxSqrDist)
                    {
                        ReturnProjectile(proj);
                    }
                }
            }
        }
    }
}