using UnityEngine;

public class AnimSpeedDriver : MonoBehaviour
{
    [Range(0f, 1f)] public float speed01 = 0f;
    Animator anim;

    void Awake() => anim = GetComponent<Animator>();

    void Update()
    {
        anim.SetFloat("Speed", speed01);
    }
}
