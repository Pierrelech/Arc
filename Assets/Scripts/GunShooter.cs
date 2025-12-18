using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GunShooter : MonoBehaviour
{
    [Header("XR")]
    private XRGrabInteractable grabInteractable;

    [Header("Tir")]
    public Transform muzzleTransform;
    public float fireRate = 0.2f;
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
            grabInteractable.activated.AddListener(OnActivated);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.activated.RemoveListener(OnActivated);
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

        // --- MODIFICATION TASK 2 : Utilisation du Pool ---
        if (ProjectilePool.Instance != null)
        {
            // 1. On récupère une balle du Pool (elle est activée automatiquement par le pool)
            GameObject bullet = ProjectilePool.Instance.GetProjectile();

            // 2. On la positionne au canon
            bullet.transform.position = muzzleTransform.position;
            bullet.transform.rotation = muzzleTransform.rotation;

            // 3. On appelle la fonction Launch du script Projectile
            Projectile projScript = bullet.GetComponent<Projectile>();
            if (projScript != null)
            {
                // Très important : Launch définit la spawnPosition utilisée par le Pool pour le nettoyage
                projScript.Launch(muzzleTransform.forward, bulletSpeed);
            }
        }
        else
        {
            Debug.LogWarning("Attention : ProjectilePool.Instance est introuvable dans la scène !");
        }
    }
}