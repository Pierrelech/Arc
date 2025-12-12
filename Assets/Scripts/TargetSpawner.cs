using System.Collections;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
<<<<<<< Updated upstream
    public GameObject cible;  // Le prefab de la cible
    public Transform player;  // Le joueur autour duquel les cibles apparaissent
    public float spawnRadius = 10f;  // Rayon autour du joueur
    public float spawnHeight = 5f;   // Hauteur � laquelle les cibles apparaissent
    private List<GameObject> activeCibles = new List<GameObject>(); // Liste des cibles actives
    private int maxCibles = 5;
=======
    public Transform player;           // Le joueur autour duquel les cibles apparaissent
    public float spawnRadius = 10f;    // Rayon autour du joueur
    public float spawnHeight = 5f;     // Hauteur à laquelle les cibles apparaissent

    public float spawnInterval = 1.0f; // Temps entre deux tentatives de spawn
>>>>>>> Stashed changes

    private void Start()
    {
        // On lance une boucle de spawn régulière
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while (true)
        {
            yield return wait;

            // sécurité : s'il manque des singletons
            if (TargetPool.Instance == null || GameplayManager.Instance == null)
                continue;

            // Est-ce qu'on a le droit de spawn une nouvelle cible ?
            if (!GameplayManager.Instance.CanSpawnTarget())
                continue;

            SpawnTarget();
        }
    }

    private void SpawnTarget()
    {
<<<<<<< Updated upstream
        // G�n�re une position al�atoire autour du joueur
=======
        // Récupère une cible dans le pool
        GameObject newCible = TargetPool.Instance.GetTarget();
        if (newCible == null)
        {
            Debug.LogWarning("TargetPool n'a pas de cible dispo !");
            return;
        }

        // Génère une position aléatoire autour du joueur
>>>>>>> Stashed changes
        Vector3 randomPos = player.position + (Random.insideUnitSphere * spawnRadius);
        randomPos.y = spawnHeight;  // Place la cible en hauteur

        // Instancie la cible
        GameObject newCible = Instantiate(cible, randomPos, Quaternion.identity);

        // Oriente la cible vers le joueur
        newCible.transform.LookAt(player);

        // Active la cible (au cas où le pool ne l’a pas déjà activée)
        newCible.SetActive(true);

<<<<<<< Updated upstream
    public void OnCibleDestroyed(GameObject cibleDetruite)
    {
        // Retire la cible d�truite de la liste
        activeCibles.Remove(cibleDetruite);

        // R�instancie la cible apr�s 1 seconde
        StartCoroutine(RespawnTarget());
    }

    IEnumerator RespawnTarget()
    {
        yield return new WaitForSeconds(1f);
        SpawnTarget();
=======
        // 🔹 On prévient le GameplayManager qu'une cible de plus est active
        GameplayManager.Instance.RegisterTargetSpawn();
>>>>>>> Stashed changes
    }
}
