using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public Transform player;      // Le joueur autour duquel les cibles apparaissent
    public float spawnRadius = 10f;  // Rayon autour du joueur
    public float spawnHeight = 5f;   // Hauteur à laquelle les cibles apparaissent
    public int maxCibles = 5;        // Nombre max de cibles en même temps
    public float respawnDelay = 1f;  // Délai avant réapparition

    private List<GameObject> activeCibles = new List<GameObject>();

    void Start()
    {
        // Crée les cibles initiales
        for (int i = 0; i < maxCibles; i++)
        {
            SpawnTarget();
        }
    }

    void SpawnTarget()
    {
        if (TargetPool.Instance == null)
        {
            Debug.LogWarning("TargetPool non présent dans la scène !");
            return;
        }

        // Récupère une cible dans le pool
        GameObject newCible = TargetPool.Instance.GetTarget();

        // Génère une position aléatoire autour du joueur
        Vector3 randomPos = player.position + (Random.insideUnitSphere * spawnRadius);
        randomPos.y = spawnHeight;  // Place la cible en hauteur

        newCible.transform.position = randomPos;

        // Oriente la cible vers le joueur
        newCible.transform.LookAt(player);

        // Ajoute la cible à la liste des cibles actives
        activeCibles.Add(newCible);
    }

    public void OnCibleDestroyed(GameObject cibleDetruite)
    {
        // Retire la cible de la liste des actives
        activeCibles.Remove(cibleDetruite);

        // Réapparition après un délai
        StartCoroutine(RespawnTarget());
    }

    IEnumerator RespawnTarget()
    {
        yield return new WaitForSeconds(respawnDelay);

        // On respawn seulement si on n’a pas déjà atteint le max
        if (activeCibles.Count < maxCibles)
        {
            SpawnTarget();
        }
    }
}
