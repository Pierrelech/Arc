using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FXAddressableManager : MonoBehaviour
{
    public static FXAddressableManager Instance;

    [Header("PCVR FX (Windows)")]
    public AssetReferenceGameObject muzzleFlashPCVR;
    public AssetReferenceGameObject impactPCVR;

    [Header("Quest FX (Android)")]
    public AssetReferenceGameObject muzzleFlashQuest;
    public AssetReferenceGameObject impactQuest;

    private GameObject muzzleFlashPrefab;
    private GameObject impactPrefab;

    private AsyncOperationHandle<GameObject> muzzleHandle;
    private AsyncOperationHandle<GameObject> impactHandle;

    private bool isQuest;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Si tu veux qu'il survive aux changements de scène :
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Detection plateforme runtime
        isQuest = (Application.platform == RuntimePlatform.Android);

        // PCVR = WindowsPlayer / WindowsEditor
        // Quest = Android

        StartCoroutine(LoadFXForPlatform());
    }

    private IEnumerator LoadFXForPlatform()
    {
        AssetReferenceGameObject muzzleRef = isQuest ? muzzleFlashQuest : muzzleFlashPCVR;
        AssetReferenceGameObject impactRef = isQuest ? impactQuest : impactPCVR;

        // Chargement async du muzzle flash
        muzzleHandle = muzzleRef.LoadAssetAsync<GameObject>();
        yield return muzzleHandle;
        if (muzzleHandle.Status == AsyncOperationStatus.Succeeded)
        {
            muzzleFlashPrefab = muzzleHandle.Result;
        }
        else
        {
            Debug.LogError("Échec du chargement du muzzle flash Addressable");
        }

        // Chargement async de l'impact
        impactHandle = impactRef.LoadAssetAsync<GameObject>();
        yield return impactHandle;
        if (impactHandle.Status == AsyncOperationStatus.Succeeded)
        {
            impactPrefab = impactHandle.Result;
        }
        else
        {
            Debug.LogError("Échec du chargement de l'impact FX Addressable");
        }
    }

    /// <summary>
    /// Joue un muzzle flash à la position / rotation d'un transform (canon).
    /// </summary>
    public void PlayMuzzleFlash(Transform muzzleTransform)
    {
        if (muzzleFlashPrefab == null) return;

        GameObject fx = Instantiate(
            muzzleFlashPrefab,
            muzzleTransform.position,
            muzzleTransform.rotation
        );

        // Si c'est un ParticleSystem, on le détruit après sa durée
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(fx, 2f);
        }
    }

    /// <summary>
    /// Joue un FX d'impact (explosion, impact balle, etc.) à une position donnée.
    /// </summary>
    public void PlayImpactFX(Vector3 position, Quaternion rotation)
    {
        if (impactPrefab == null) return;

        GameObject fx = Instantiate(impactPrefab, position, rotation);

        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(fx, 2f);
        }
    }

    /// <summary>
    /// À appeler si tu veux libérer la mémoire (ex: changement de scène, fin de jeu).
    /// </summary>
    public void UnloadFX()
    {
        if (muzzleHandle.IsValid())
        {
            Addressables.Release(muzzleHandle);
        }
        if (impactHandle.IsValid())
        {
            Addressables.Release(impactHandle);
        }

        muzzleFlashPrefab = null;
        impactPrefab = null;
    }

    private void OnDestroy()
    {
        // Pour éviter les leaks si l'objet est détruit
        UnloadFX();
    }
}
