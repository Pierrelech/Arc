using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 40f;
    public float maxLifeTime = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // La balle avance tout droit selon son forward
        rb.linearVelocity = transform.forward * speed;

        // On la d�truit si elle ne touche rien au bout d'un moment
        Destroy(gameObject, maxLifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Tu peux mettre des effets ici, d�g�ts etc.
        Debug.Log("Balle a touch� : " + collision.gameObject.name);

        // Ensuite on d�truit la balle
        Destroy(gameObject);
    }
}
