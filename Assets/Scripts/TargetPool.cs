using System.Collections.Generic;
using UnityEngine;

public class TargetPool : MonoBehaviour
{
    public static TargetPool Instance;

    [Header("Pool de cibles")]
    public GameObject targetPrefab;   // Ton prefab de cible
    public int initialSize = 10;      // Nombre de cibles pré-instanciées

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
        // Pré-instanciation
        for (int i = 0; i < initialSize; i++)
        {
            GameObject cible = Instantiate(targetPrefab, transform);
            cible.SetActive(false);
            pool.Enqueue(cible);
        }
    }

    /// <summary>
    /// Récupère une cible (réutilisation prioritaire, instantiation seulement si nécessaire).
    /// </summary>
    public GameObject GetTarget()
    {
        GameObject cible;

        if (pool.Count > 0)
        {
            cible = pool.Dequeue();
        }
        else
        {
            cible = Instantiate(targetPrefab, transform);
        }

        // Réinitialise l'état de la cible
        Cible cibleScript = cible.GetComponent<Cible>();
        if (cibleScript != null)
        {
            cibleScript.ResetState();
        }

        cible.SetActive(true);
        return cible;
    }

    /// <summary>
    /// Remet une cible dans le pool (désactivation + ajout à la queue).
    /// </summary>
    public void ReturnTarget(GameObject cible)
    {
        cible.SetActive(false);
        pool.Enqueue(cible);
    }
}
