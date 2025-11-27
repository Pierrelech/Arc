using System.Collections;
using UnityEngine;

public class Cible : MonoBehaviour
{
    private TargetSpawner spawner;

    [Header("FX")]
    public AudioSource audioExplode;
    public GameObject explosionEffect;
    public float explosionDuration = 2f;

    [Header("Reset Visuel")]
    private Renderer rend;
    private Color baseColor;
    private Collider coll;

    void Awake()
    {
        spawner = FindObjectOfType<TargetSpawner>();
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
        // ✅ Remise à zéro complète à chaque réapparition (TASK 2)
        ResetState();

        // ✅ Son à l’apparition (facultatif)
        if (audioExplode != null)
        {
            audioExplode.Play();
        }
    }

    // ✅ RESET COMPLET DE LA CIBLE
    public void ResetState()
    {
        if (rend != null)
        {
            rend.material.color = baseColor;
        }

        if (coll != null)
        {
            coll.enabled = true;
        }
    }

    // ✅ DÉTECTION DES IMPACTS
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow") ||
            collision.gameObject.CompareTag("Bullet"))
        {
            Hit();
        }
    }

    // ✅ LOGIQUE DE DESTRUCTION (SANS DESTROY)
    public void Hit()
    {
        if (audioExplode != null)
            audioExplode.Play();

        TriggerExplosion();

        if (coll != null)
            coll.enabled = false;

        StartCoroutine(ReturnToPoolAfterDelay(0.1f));
    }

    IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ✅ Informe le spawner
        if (spawner != null)
            spawner.OnCibleDestroyed(gameObject);

        // ✅ Retour dans le pool (AU LIEU DE Destroy)
        if (TargetPool.Instance != null)
        {
            TargetPool.Instance.ReturnTarget(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // ✅ EXPLOSION VISUELLE
    void TriggerExplosion()
    {
        if (FXAddressableManager.Instance != null)
        {
            FXAddressableManager.Instance.PlayImpactFX(transform.position, transform.rotation);
        }
    }

}
