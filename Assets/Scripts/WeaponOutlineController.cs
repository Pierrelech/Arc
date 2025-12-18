using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WeaponOutlineController : MonoBehaviour
{
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private XRGrabInteractable _interactable;

    void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        _interactable = GetComponent<XRGrabInteractable>();

        // On initialise le halo à "éteint"
        UpdateOutline(0, 0);

        _interactable.hoverEntered.AddListener(args => UpdateOutline(1f, 0f)); // Hover
        _interactable.hoverExited.AddListener(args => UpdateOutline(0f, 0f));  // Off
        _interactable.selectEntered.AddListener(args => UpdateOutline(1f, 1f)); // Grab
        _interactable.selectExited.AddListener(args => UpdateOutline(1f, 0f));  // Retour au Hover
    }

    private void UpdateOutline(float hoverVal, float grabVal)
    {
        // On cible spécifiquement l'Element 1 (le halo) sans toucher à l'Element 0
        _renderer.GetPropertyBlock(_propBlock, 1);
        _propBlock.SetFloat("_Hover", hoverVal);
        _propBlock.SetFloat("_Grab", grabVal);
        _renderer.SetPropertyBlock(_propBlock, 1);
    }
}