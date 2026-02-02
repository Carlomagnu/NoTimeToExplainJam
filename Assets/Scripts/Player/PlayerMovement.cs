using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float playerSpeed;
    [SerializeField] Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    [SerializeField] float groundFriction;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMultiplier;
    bool canJump = true;

    [Header("SlopeMovement")]
    [SerializeField] float maxSlopeAngle;
    [SerializeField] private RaycastHit slopeHit;
    private bool exitingSlope;



    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = groundCheck();
        getInput();
        movePlayer();
        applyFriction();
        speedControl();
    }

    //Get WASD
    private void getInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.Space) && canJump && isGrounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void movePlayer()
    {
        // Calculate movememnt direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On slope
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * playerSpeed * 10f, ForceMode.Force);
        }

        // On the ground
        else if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * playerSpeed * 10, ForceMode.Force);
        }
        // In the air
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * playerSpeed * 10 * airMultiplier, ForceMode.Force);
        }

        //Turn off gravity so we dont slide down
        rb.useGravity = !OnSlope();
        
    }

    // Detects whether player is grounded
    private bool groundCheck()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
    }

    private void applyFriction()
    {
        if (isGrounded)
        {
            rb.drag = groundFriction;
        }
        else
        {
            rb.drag = 0;
        }
    }

    // Limit velocity if needed
    private void speedControl()
    {
        if (OnSlope())
        {
            if (rb.velocity.magnitude > playerSpeed)
            {
                rb.velocity = rb.velocity.normalized * playerSpeed;
            }
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > playerSpeed)
            {
                Vector3 cappedVel = flatVel.normalized * playerSpeed;
                rb.velocity = new Vector3(cappedVel.x, rb.velocity.y, cappedVel.z);
            }
        }

    }

    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
        exitingSlope = false;
    }

    //Slope movement
    private bool OnSlope()
    {
        if (exitingSlope)
        {
            return false;
        }
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f, groundLayer))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }
}
