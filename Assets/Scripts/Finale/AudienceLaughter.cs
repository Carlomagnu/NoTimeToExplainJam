using UnityEngine;

public class AudienceMovement : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Randomize animation speed so the crowd feels alive
        animator.SetFloat("Speed", Random.Range(0.8f, 1.2f));

        // Always moving
        animator.SetBool("Move", true);
    }
}

