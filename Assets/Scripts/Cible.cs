using UnityEngine;

public class Cible : MonoBehaviour
{
    private TargetSpawner spawner;
    public AudioSource audioExplode;
    public GameObject explosionEffect;  // Référence au prefab du Particle System
    public float explosionDuration = 2f;  // Durée de l'explosion

    void Start()
    {
        spawner = FindObjectOfType<TargetSpawner>();
        audioExplode = GetComponent<AudioSource>();

        // Joue le son à l'apparition
        if (audioExplode != null)
        {
            audioExplode.Play();
        }
    }

    // Détection des collisions
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow") || collision.gameObject.CompareTag("Bullet"))
        {
            
            audioExplode.Play();
            
            // Déclenche l'effet de particules
            TriggerExplosion();


            // Détruit la cible
            Destroy(gameObject);

            // Informe le spawner qu'une cible a été détruite
            spawner.OnCibleDestroyed(gameObject);
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
