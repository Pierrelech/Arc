using UnityEngine;

public class Cible : MonoBehaviour
{
<<<<<<< Updated upstream
    private TargetSpawner spawner;
=======
    [Header("FX")]
>>>>>>> Stashed changes
    public AudioSource audioExplode;
    public GameObject explosionEffect;  // Référence au prefab du Particle System
    public float explosionDuration = 2f;  // Durée de l'explosion

<<<<<<< Updated upstream
    void Start()
    {
        spawner = FindObjectOfType<TargetSpawner>();
        audioExplode = GetComponent<AudioSource>();

        // Joue le son à l'apparition
=======
    [Header("Reset Visuel")]
    private Renderer rend;
    private Color baseColor;
    private Collider coll;

    private GameplayManager gameplayManager;

    void Awake()
    {
        // RÃ©fÃ©rence vers le GameplayManager
        gameplayManager = GameplayManager.Instance;

        if (audioExplode == null)
            audioExplode = GetComponent<AudioSource>();

        coll = GetComponent<Collider>();

        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            baseColor = rend.material.color;
        }
    }

    void OnEnable()
    {
        // âœ… Remise Ã  zÃ©ro complÃ¨te Ã  chaque rÃ©apparition (TASK 2)
        ResetState();

        // (Optionnel) Son Ã  lâ€™apparition
        // Si tu veux garder le son seulement Ã  l'impact, commente Ã§a
>>>>>>> Stashed changes
        if (audioExplode != null)
        {
             audioExplode.Play();
        }
    }

    // âœ… Quand la cible est dÃ©sactivÃ©e (touchÃ©e / reset / sortie Ã©cran / etc.)
    private void OnDisable()
    {
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.RegisterTargetDespawn();
        }
    }

    // Détection des collisions
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow") || collision.gameObject.CompareTag("Bullet"))
        {
<<<<<<< Updated upstream
            
=======
            Hit();
        }
    }

    // âœ… LOGIQUE DE â€œDESTRUCTIONâ€ (retour au pool)
    public void Hit()
    {
        if (audioExplode != null)
>>>>>>> Stashed changes
            audioExplode.Play();
            
            // Déclenche l'effet de particules
            TriggerExplosion();


<<<<<<< Updated upstream
            // Détruit la cible
            Destroy(gameObject);

            // Informe le spawner qu'une cible a été détruite
            spawner.OnCibleDestroyed(gameObject);
=======
        // SÃ©curitÃ© si jamais GameplayManager a Ã©tÃ© initialisÃ© aprÃ¨s
        if (gameplayManager == null)
            gameplayManager = GameplayManager.Instance;

        if (gameplayManager != null)
            gameplayManager.AddScore(1);

        if (coll != null)
            coll.enabled = false;

        StartCoroutine(ReturnToPoolAfterDelay(0.1f));
    }

    IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // âœ… Retour dans le pool
        if (TargetPool.Instance != null)
        {
            TargetPool.Instance.ReturnTarget(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
>>>>>>> Stashed changes
        }
    }

    // Méthode pour déclencher l'explosion
    void TriggerExplosion()
    {
        // Instancie l'effet d'explosion à la position de la cible
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(explosion, explosionDuration);  // Détruit l'explosion après un délai
        }
    }
}
