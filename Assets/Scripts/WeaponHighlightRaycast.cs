using UnityEngine;

public class WeaponHighlightRaycast : MonoBehaviour
{
    [SerializeField] private Camera rayCamera;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private LayerMask weaponLayer;

    private static readonly int HoverId = Shader.PropertyToID("_Hover");
    private static readonly int GrabId = Shader.PropertyToID("_Grab");

    private MaterialPropertyBlock mpb;
    private Renderer current;

    private float hoverValue, grabValue;
    private float hoverTarget, grabTarget;

    private void Awake()
    {
        if (rayCamera == null) rayCamera = Camera.main;
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        // Raycast centre écran
        Ray ray = new Ray(rayCamera.transform.position, rayCamera.transform.forward);

        Renderer hitRend = null;
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, weaponLayer))
            hitRend = hit.collider.GetComponentInParent<Renderer>();

        // Hover
        if (hitRend != null)
        {
            SetHover(hitRend, 1f);
            if (current != null && current != hitRend) SetHover(current, 0f);
            current = hitRend;
        }
        else if (current != null)
        {
            SetHover(current, 0f);
            current = null;
        }

        // Grab (exemple clavier: E) -> remplace par input VR
        grabTarget = Input.GetKey(KeyCode.E) ? 1f : 0f;

        // Smooth sur l’objet current
        if (current != null)
        {
            hoverValue = Mathf.Lerp(hoverValue, hoverTarget, 1f - Mathf.Exp(-12f * Time.deltaTime));
            grabValue = Mathf.Lerp(grabValue, grabTarget, 1f - Mathf.Exp(-18f * Time.deltaTime));

            current.GetPropertyBlock(mpb);
            mpb.SetFloat(HoverId, hoverValue);
            mpb.SetFloat(GrabId, grabValue);
            current.SetPropertyBlock(mpb);
        }
    }

    private void SetHover(Renderer r, float t)
    {
        hoverTarget = t;
        r.GetPropertyBlock(mpb);
        mpb.SetFloat(HoverId, t);
        r.SetPropertyBlock(mpb);
    }
}
