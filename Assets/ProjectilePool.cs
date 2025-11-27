using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    [Header("Pool de projectiles")]
    public GameObject projectilePrefab;   // Mets ici ton prefab de balle (le même que dans ton Gun avant)
    public int initialSize = 20;          // Nombre de projectiles pré-instanciés

    private Queue<GameObject> pool = new Queue<GameObject>();

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
    }

    public GameObject GetProjectile()
    {
        GameObject proj;

        if (pool.Count > 0)
        {
            proj = pool.Dequeue();
        }
        else
        {
            proj = Instantiate(projectilePrefab, transform);
        }

        proj.SetActive(true);
        return proj;
    }

    public void ReturnProjectile(GameObject proj)
    {
        proj.SetActive(false);
        pool.Enqueue(proj);
    }
}
