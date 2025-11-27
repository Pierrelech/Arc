using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GunShooter : MonoBehaviour
{
    [Header("XR")]
    private XRGrabInteractable grabInteractable;

    [Header("Tir")]
    public Transform muzzleTransform;           // point de sortie du tir (bout du canon)
    public float fireRate = 0.2f;               // cadence (en secondes entre 2 tirs)
    public float damage = 10f;
    public float range = 100f;
    public LayerMask hitMask = ~0;             // sur quoi le rayon peut toucher

    [Header("Projectile (optionnel)")]
    public GameObject bulletPrefab;            // si tu veux instancier un projectile
    public float bulletSpeed = 30f;

    [Header("FX")]
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shotClip;

    private float lastShotTime = 0f;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            // Appel� quand tu appuies sur la g�chette (Activate)
            grabInteractable.activated.AddListener(OnActivated);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.activated.RemoveListener(OnActivated);
        }
    }

    private void OnActivated(ActivateEventArgs args)
    {
        TryShoot();
    }

    private void TryShoot()
    {
        if (Time.time - lastShotTime < fireRate)
            return;

        lastShotTime = Time.time;
        Shoot();
    }

    private void Shoot()
    {
        // FX
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }


        if (audioSource != null && shotClip != null)
            audioSource.PlayOneShot(shotClip);

        // Si tu as un prefab de balle, on l'instancie
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzleTransform.position, muzzleTransform.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = muzzleTransform.forward * bulletSpeed;
            }
        }
        else
        {
            // Sinon : tir en raycast (hitscan)
            Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
            {
                // Debug : voir le tir dans la Scene view
                Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);

                // Chercher un script "Health" sur la cible (si tu en as un)

            }
        }
    }
}
