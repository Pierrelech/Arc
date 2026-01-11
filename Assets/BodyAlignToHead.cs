using UnityEngine;

public class BodyAlignToHead : MonoBehaviour
{
    public Transform headAnchor;
    public Transform hmd;
    public CharacterController characterController;
    public Animator animator; // Ajoute l'Animator ici

    [Header("Animation Settings")]
    public float speedThreshold = 0.1f; // Vitesse minimum pour animer

    void LateUpdate()
    {
        if (!headAnchor || !hmd || !characterController || !animator) return;

        // --- 1. POSITION ---
        Vector3 headPositionXZ = new Vector3(headAnchor.position.x, 0, headAnchor.position.z);
        Vector3 rootPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 offsetXZ = rootPositionXZ - headPositionXZ;

        transform.position = new Vector3(hmd.position.x, transform.position.y, hmd.position.z) + offsetXZ;

        // --- 2. ROTATION ---
        Vector3 forward = hmd.forward;
        forward.y = 0;
        if (forward.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(forward);
        }

        // --- 3. CHARACTER CONTROLLER & ANIMATION ---
        float newHeight = hmd.localPosition.y;
        characterController.height = newHeight;
        characterController.center = new Vector3(0, newHeight / 2f, 0);

        // Récupérer la vitesse horizontale du Character Controller
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        // Envoyer la vitesse à l'Animator (avec un petit seuil pour éviter les tremblements)
        if (currentSpeed > speedThreshold)
        {
            animator.SetFloat("Speed", currentSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }
}