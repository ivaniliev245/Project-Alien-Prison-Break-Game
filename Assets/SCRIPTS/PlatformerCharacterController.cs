using UnityEngine;
using UnityEngine.AI;

public class PlatformerCharacterController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float walkAnimationSpeedMultiplier = 1.0f;
    public float runAnimationSpeedMultiplier = 1.5f;
    public bool lockedRotation = true;
    public float lockedRotationY = 0f;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool grounded;
    private float jumpTimer;

    private Transform mainCamera;
    public float cameraFollowSpeed = 5.0f;
    public Vector3 cameraOffset = new Vector3(0, 2, -3);

    private Animator animator;
    private const float locomotionSmoothTime = 0.1f;
    private bool hasAnimator;

    public float gravity = -9.81f;
    public float jumpForce = 20f;
    public float jumpCooldown = 0.25f;

    const float threshold = 0.01f;
    NavMeshAgent agent;
    private bool isRunning = false;

    // Add the jump animation parameter
    private bool isJumping = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        jumpTimer = 0f;
        mainCamera = Camera.main.transform;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        hasAnimator = animator != null;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool runInput = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed = runInput ? runSpeed : walkSpeed;
        float currentAnimationSpeedMultiplier = runInput ? runAnimationSpeedMultiplier : walkAnimationSpeedMultiplier;

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        if (grounded && jumpTimer <= 0f)
        {
            movement = transform.TransformDirection(movement);
            movement.Normalize();
            velocity = movement * currentSpeed;

            // Set the "Speed" parameter in the Animator
            if (hasAnimator)
            {
                animator.SetFloat("Speed", velocity.magnitude * currentAnimationSpeedMultiplier, locomotionSmoothTime, Time.deltaTime);
                isJumping = false; // Reset the jump parameter
            }
        }

        velocity.y += gravity * Time.deltaTime;
        grounded = characterController.isGrounded;

        // Handle jumping animation
        if (!isJumping && Input.GetButtonDown("Jump") && grounded && jumpTimer <= 0f)
        {
            velocity.y = jumpForce;
            jumpTimer = jumpCooldown;
            isJumping = true;
        }

        // Set the "Jump" parameter in the Animator
        if (hasAnimator)
        {
            animator.SetBool("Jump", isJumping);
        }

        if (velocity.y > jumpForce)
        {
            velocity.y = jumpForce;
        }

        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);

        UpdateRotation();

        if (mainCamera != null)
        {
            Vector3 cameraTargetPosition = transform.position + cameraOffset;
            mainCamera.position = Vector3.Lerp(mainCamera.position, cameraTargetPosition, Time.deltaTime * cameraFollowSpeed);
        }
    }

    void UpdateRotation()
    {
        if (lockedRotation)
        {
            transform.rotation = Quaternion.Euler(0f, lockedRotationY, 0f);
        }
        else
        {
            transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X"), 0f));
        }
    }
}