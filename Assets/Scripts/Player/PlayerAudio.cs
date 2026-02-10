using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Footsteps")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepInterval = 0.5f;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private float minMoveSpeed = 0.1f;

    [Header("Jump / Land")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;

    private Rigidbody rb;
    private float footstepTimer;
    private bool wasMovingLastFrame;

    //Jumping logic
    private bool wasGroundedLastFrame;
    private float verticalVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleFootsteps();
        HandleJumpLand();
    }

    void Awake()
    {
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
        

    }

    // ---------------- FOOTSTEPS ----------------

    public void HandleFootsteps()
    {
        if (!movement.isGrounded)
        {
            footstepTimer = 0f;
            wasMovingLastFrame = false;
            return;
        }

        Vector3 horizontalVel = rb.velocity;
        horizontalVel.y = 0f;

        bool isMoving = horizontalVel.magnitude > minMoveSpeed;

        // If we JUST stopped moving, kill timer
        if (!isMoving)
        {
            footstepTimer = 0f;
            wasMovingLastFrame = false;
            return;
        }

        // If we JUST started moving, play instantly
        if (!wasMovingLastFrame)
        {
            PlayFootstep();
            footstepTimer = 0f;
        }

        footstepTimer += Time.deltaTime;

        if (footstepTimer >= footstepInterval)
        {
            PlayFootstep();
            footstepTimer = 0f;
        }

        wasMovingLastFrame = true;
    }

    private void PlayFootstep()
    {
        Debug.Log("Plaing footstep");
        if (footstepClips.Length == 0) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];

        audioSource.pitch = Random.Range(0.6f, 1f);
        audioSource.PlayOneShot(clip, 0.8f);
    }

    // Jumping and Landing

    public void PlayJump()
    {
        if (!jumpClip) return;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(jumpClip);
    }

    public void PlayLand()
    {
        if (!landClip) return;

        audioSource.pitch = Random.Range(0.9f, 1.0f);
        audioSource.PlayOneShot(landClip);
    }

    private void HandleJumpLand()
    {
        verticalVelocity = rb.velocity.y;

        if (wasGroundedLastFrame && !movement.isGrounded && verticalVelocity > 0.1f)
        {
            PlayJump();
        }

 
        if (!wasGroundedLastFrame && movement.isGrounded && verticalVelocity < -1f)
        {
            PlayLand();
        }

        wasGroundedLastFrame = movement.isGrounded;
    }
}
