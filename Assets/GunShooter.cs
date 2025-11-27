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
    public LayerMask hitMask = ~0;              // sur quoi le rayon peut toucher

    [Header("Projectile (pool)")]
    public float bulletSpeed = 30f;             // vitesse des projectiles

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
        // FX visuel via Addressables
        if (FXAddressableManager.Instance != null && muzzleTransform != null)
        {
            FXAddressableManager.Instance.PlayMuzzleFlash(muzzleTransform);
        }


        if (audioSource != null && shotClip != null)
            audioSource.PlayOneShot(shotClip);

        // --- TIR AVEC PROJECTILE DU POOL ---
        if (ProjectilePool.Instance != null)
        {
            GameObject bullet = ProjectilePool.Instance.GetProjectile();

            // Position & rotation au niveau du canon
            bullet.transform.position = muzzleTransform.position;
            bullet.transform.rotation = muzzleTransform.rotation;

            // Lancer le projectile
            Projectile proj = bullet.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Launch(muzzleTransform.forward, bulletSpeed);
            }
        }
        else
        {
            // Fallback : raycast (au cas où le pool n'est pas dans la scène)
            Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);

                // Gestion des dégâts si tu as un script Health
            }
        }
    }
}
