using UnityEngine;

public class RandomAnimationSpeed : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed *= Random.Range(0.25f, 0.75f);
    }
}
