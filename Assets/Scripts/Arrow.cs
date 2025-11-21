using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrows : MonoBehaviour
{
    public float speed = 10f;
    public Transform tip;

    private Rigidbody _rigidBody;
    private bool _inAir = false;
    private Vector3 _lastPosition = Vector3.zero;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        PullInteraction.PullActionReleased += Release;

        Stop();
    }

    private void OnDestroy()
    {
        PullInteraction.PullActionReleased -= Release;
    }

    private void Release(float value)
    {
        PullInteraction.PullActionReleased -= Release;
        gameObject.transform.parent = null;
        _inAir = true;
        SetPhysics(true);

        Vector3 force = transform.forward * value * speed;
        _rigidBody.AddForce(force, ForceMode.Impulse);

        StartCoroutine(RotateWithVelocity());

        _lastPosition = tip.position;
    }

    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (_inAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(_rigidBody.linearVelocity, transform.up);
            transform.rotation = newRotation;
            yield return null;
        }
    }


    private void FixedUpdate()
    {
        if (_inAir)
        {
            CheckCollision();
            _lastPosition = tip.position;
        }


    }

    private void CheckCollision()
    {
        if (Physics.Linecast(_lastPosition, tip.position, out RaycastHit hitInfo))
        {
            // V�rifie que l'objet touch� n'a pas le tag "Cible"
            if (!hitInfo.transform.CompareTag("Cible"))
            {
                // Si l'objet a un Rigidbody, applique une force � l'objet touch�
                if (hitInfo.transform.TryGetComponent(out Rigidbody body))
                {
                    _rigidBody.interpolation = RigidbodyInterpolation.None;
                    transform.parent = hitInfo.transform;
                    body.AddForce(_rigidBody.linearVelocity, ForceMode.Impulse);
                }

                // Plante la fl�che � la position exacte du point d'impact
                transform.position = hitInfo.point - tip.localPosition;
                transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);

                // Stoppe la fl�che
                Stop();
                Destroy(gameObject);
            }
            else
            {
                // Si l'objet est une cible, ne plante pas la fl�che
                Debug.Log("Collision avec une cible ignor�e.");
            }
        }
    }




    private void Stop()
    {
        _inAir = false;
        SetPhysics(false);
    }


    private void SetPhysics(bool usePhysics)
    {
        _rigidBody.useGravity = usePhysics;
        _rigidBody.isKinematic = !usePhysics;
    }


}