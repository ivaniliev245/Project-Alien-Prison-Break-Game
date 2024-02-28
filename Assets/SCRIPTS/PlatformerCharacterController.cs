using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Cinemachine;

public class PlatformerCharaterController : MonoBehaviour
{
    //movement and rotation
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float walkAnimationSpeedMultiplier = 1.0f;
    public float runAnimationSpeedMultiplier = 1.5f;
    public bool lockedRotation = true;
    public float lockedRotationY = 0f;
    public float rotationSpeed = 4;
    public float restrictZmovment = 1;

    private CharacterController characterController;
    public float newHeight = 0.3f; // For Crouching
    private float oldHeight;
    public float newCenterY = -0.72f;// For Crouching
    private Vector3 originalLifePosition;// For Crouching
    private Vector3 newLifePosition;// For Crouching
    private Vector3 oldCenter;// For Crouching
    private Vector3 velocity;
    private bool grounded;
    public float coyoteTime = 0.15f;
    public float jumpInputTimeBuffer = 0.1f;
    
    public float idleSpeed = 0.1f;


    //float timers
    private float lastOnGroundTime = 0f;
    private float lastPressedJumpTime = 0f;
  
    //handle cinemachine camera
    public CinemachineVirtualCamera virtualCamera;
    
    // animation values 
    private Animator animator;
    private const float locomotionSmoothTime = 0.1f;
    private bool hasAnimator;

    public Transform childObject;
    public GameObject Life;// Reference to the child object to rotate
     // Adjustable rotation speed
    
    public float gravity = -9.81f;
    public float jumpForce = 20f;

    private const float threshold = 0.01f;
    private NavMeshAgent agent;
   

    // Add the jump animation parameter
    private bool IsSliding; //check for wall sliding
   
    //crouch parameter
    private bool isCrouching = false;
   
    private bool isAttacking = false;
    private string[] attackAnimations = 
    { "AttackR", "AttackL", "AttackL_v2", "AttackR_v2", "StrongAttack" };
    
    // handle crouching 
    private const string crouchNoWalkParam = "crouchNoWalk";  
  
    // Update Rotation
    public float followAxis = 0.0f;
   
    //handle jump duration
    public float jumpDuration = 0.5f;

    public float maxHealth;
    private float currentHealth;
    [SerializeField] private HealthbarPlayer healthbar;

    public float invincibilityLength;
    private float invincibilityCounter;

    public GameObject objectToDestroy;
    public GameObject deathSplashPrefab;
    public GameOverScreen gameOverScreen;
    public float gameOverDelay = 2.0f;

    public Transform deathSplashLocation;

    [Header("Fall Damage")]
    
    public float fallDamageHeightThreshold = 10f; // Adjust as needed
    public float fallDamageMultiplier = 1f; // Adjust as needed
    public int baseFallDamage = 10; // Adjust as needed

    private float fallStartHeight;
    

    [Header("Prevent Climbing Slopes")]
     public float maxSlopeAngle = 45f;
     public float slideSpeed = 5f;

    

    void Start()
    {
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        hasAnimator = animator != null;
        originalLifePosition = Life.transform.position;

        currentHealth = maxHealth;
        Life.SetActive(false);
    }

    void Update()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;

        if (invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool runInput = Input.GetKey(KeyCode.LeftShift);

        // ensure no micro movement
        if (Mathf.Abs(horizontalInput) < 0.1f && Mathf.Abs(verticalInput) < 0.1f)
        {
            horizontalInput = 0f;
            verticalInput = 0f;
        }

        float currentSpeed = runInput ? runSpeed : walkSpeed;
        float currentAnimationSpeedMultiplier = runInput ? runAnimationSpeedMultiplier : walkAnimationSpeedMultiplier;

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        bool isMoving = movement.magnitude > 0.1f;
        bool isCrouchingNoWalk = !isMoving && isCrouching; // Not moving and crouching
        bool isCrouchingNoWalkOrMove = isMoving && isCrouching; // Moving and crouching


        float currentVelocity = isMoving ? velocity.magnitude * currentAnimationSpeedMultiplier : idleSpeed; // Calculate speed based on movement or idle state


        if (Input.GetMouseButtonDown(0)) // 0 represents left mouse button, change as needed
        {
            StartAttack(); // Call method to initiate attack
        }

        if (Input.GetMouseButtonUp(0)) // 0 represents left mouse button, change as needed
        {
            StopAttack(); // Call method to stop attack
        }

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            ToggleCrouch();
        }

        if (Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.JoystickButton1))
        {
            ToggleCrouchUp();
            animator.SetBool("Crouch", false);
            animator.SetBool("crouchNoWalk", false);

        }

        grounded = characterController.isGrounded;

        if (grounded)
        {
            lastOnGroundTime = coyoteTime;
        }

        if (grounded && lastOnGroundTime > 0)
        {
            movement = transform.TransformDirection(movement);
            movement.Normalize();
            velocity = movement * currentSpeed;

            float transitionDuration = 0.0001f;

            if (hasAnimator)
            {
                if (grounded) // Only update speed if grounded
                {
                    animator.SetFloat("Speed", velocity.magnitude *
                         currentAnimationSpeedMultiplier, locomotionSmoothTime, Time.deltaTime);
                }

                animator.SetBool("crouchNoWalk", isCrouchingNoWalk);
                animator.SetBool("Crouch", isCrouchingNoWalkOrMove);
            }


        }

        CheckFallDamage();
        PerformJump();
        UpdateRotation();
        UpdateGroundedState();
        UpdateAnimatorSpeed(currentVelocity); // Update the speed parameter in the animator
        CalculateAndPrintSlopeAngle();
   
        if (!grounded && IsSlopeTooSteep())
    {
        SlideDownSlope();
    }
   
   
    }

    void UpdateRotation()
    {
        if (!lockedRotation)
        {
            // Get the input for horizontal (A and D keys) and vertical (W and S keys) movement
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Calculate the angle in radians based on the input
            float targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg;

            // If there is no input, don't update the rotation
            if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
            {
                // Convert it to Euler angles
                Vector3 newEulerAngles = new Vector3(0, targetAngle, 0);

                // Apply the adjustable follow axis
                newEulerAngles.y += followAxis;

                // Create the new rotation quaternion for the child object
                Quaternion newRotation = Quaternion.Euler(newEulerAngles);

                // Rotate the child object
                childObject.rotation = Quaternion.Slerp(childObject.rotation, newRotation, Time.deltaTime * 
rotationSpeed);
}
}
}
    void ToggleCrouch()
    {
        if (!isCrouching)
        {
            isCrouching = true;

            float transitionDuration = 0.2f;

            // Set the "crouchNoWalk" parameter to true
            if (animator != null)
            {
                animator.SetBool("crouchNoWalk", true);
            }


        oldHeight = characterController.height;
        oldCenter = characterController.center;

        characterController.height = newHeight;
        Vector3 newCenter = characterController.center;
        newCenter.y = newCenterY;
        characterController.center = newCenter;
    }
}


 void ToggleCrouchUp()
    {
        if (isCrouching)
        {
            isCrouching = false;

            float transitionDuration = 0.2f;

            // Set the "crouchNoWalk" parameter to false
            if (animator != null)
            {
                animator.SetBool("crouchNoWalk", false);
            }

        characterController.center = oldCenter;
        characterController.height = oldHeight;
    }
}

void StartAttack()
{
    isAttacking = true;

    // Get a random attack animation name from the array
    string randomAttack = attackAnimations[Random.Range(0, attackAnimations.Length)];

    // Set the random attack animation parameter to true
    if (hasAnimator)
    {
        animator.SetBool(randomAttack, true);

        // Ensure all other attack animations are set to false to prevent conflicts
        foreach (string attackAnimation in attackAnimations)
        {
            if (attackAnimation != randomAttack)
            {
                animator.SetBool(attackAnimation, false);
            }
        }
    }
}

void StopAttack()
{
    isAttacking = false;

    // Reset all attack animations by setting their boolean parameters to false
    foreach (string attackAnimation in attackAnimations)
    {
        animator.SetBool(attackAnimation, false);
    }
}

public void OnJumpInput()
{
    lastPressedJumpTime = jumpInputTimeBuffer;
}

public bool CanJump()
{
    return lastOnGroundTime > 0 && grounded && lastPressedJumpTime > 0;
}

public void Jump()
{
    //ensure the player can't jump twice
    lastPressedJumpTime = 0;
    lastOnGroundTime = 0;

    StartCoroutine(ApplyJumpForce());
}

void PerformJump()
{
    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
    {
        if (!IsSlopeTooSteep())
        {
            OnJumpInput();
        }
    }

    if (CanJump())
    {
        grounded = characterController.isGrounded;
        Jump();
    }

    velocity.y += gravity * Time.deltaTime;
    characterController.Move(velocity * Time.deltaTime);
}

IEnumerator ApplyJumpForce()
{
    float timeInAir = 0f;
    float initialJumpForce = Mathf.Sqrt(jumpForce * -2f * gravity);

    if (hasAnimator)
    {
        animator.SetBool("Jump", true);
    }

    while (timeInAir < jumpDuration) // Use adjustable jump duration
    {
        float jumpVelocity = initialJumpForce - (gravity * timeInAir);
        velocity.y = jumpVelocity;

        timeInAir += Time.deltaTime;

        yield return null;
    }

    if (hasAnimator)
    {
        animator.SetBool("Jump", false);
    }
}

void UpdateAnimatorSpeed(float speed)
{
    float minSpeed = 0.001f; // Adjust as needed
    float maxAnimatorSpeed = 1.5f; // Define the maximum speed value for the animator here

    // Ensure minimum speed threshold
    speed = Mathf.Max(speed, minSpeed);

    // Ensure speed doesn't exceed the maximum speed value for the animator
    speed = Mathf.Min(speed, maxAnimatorSpeed);

    if (hasAnimator)
    {
        animator.SetFloat("Speed", speed); // Set the speed parameter in the animator
    }
}

void UpdateGroundedState()
{
    grounded = characterController.isGrounded;
    if (grounded && lastOnGroundTime > 0)
    {
        grounded = characterController.isGrounded;

        if (hasAnimator)
        {
            animator.SetBool("Grounded", grounded);
        }
    }
    else
    {
        if (hasAnimator)
        {
            animator.SetBool("Grounded", false);
        }
    }
}

 public void TakeDamage(int damage)
    {
        if (invincibilityCounter <= 0)
        {
            currentHealth -= damage;
            invincibilityCounter = invincibilityLength;
            healthbar.UpdateHealthbar(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die( objectToDestroy);
            }
        }
    }

 private void Die(GameObject objectToDestroy)
{
    // Destroy the specific game object before replacing it
    Destroy(objectToDestroy);

    // Instantiate a new character prefab with adjustable settings
    GameObject deathSplash = Instantiate(deathSplashPrefab, deathSplashLocation.position, deathSplashLocation.rotation);
    // You may need to adjust the position and rotation as needed

    // Additional setup for the new character prefab, such as adjusting settings or adding components, can be done here

    // Invoke the game over setup with a delay
    Invoke("GameOverSetup", gameOverDelay);
}

private void GameOverSetup()
{
    // Show game over screen or perform other game over actions
    gameOverScreen.Setup();

    // Additional actions, such as stopping movement or disabling collisions, can be added here
}


public void SetVelocity(Vector3 newVelocity)
{
    velocity = newVelocity;
}

public void SetCurrentHealth(int health) { currentHealth = health; }


void CheckFallDamage()
    {
        // Check if the player is falling
        if (!characterController.isGrounded && velocity.y < 0)
        {
            if (fallStartHeight == 0)
            {
                // Record the starting height of the fall
                fallStartHeight = transform.position.y;
            }
        }
        else
        {
            if (fallStartHeight != 0)
            {
                // Calculate the distance fallen
                float fallDistance = fallStartHeight - transform.position.y;

                // Check if fall distance exceeds the threshold for taking fall damage
                if (fallDistance > fallDamageHeightThreshold)
                {
                    // Calculate fall damage
                    int damage = Mathf.RoundToInt(baseFallDamage + (fallDistance - fallDamageHeightThreshold) * fallDamageMultiplier);

                    // Apply fall damage
                    TakeDamage(damage);
                }

                // Reset fall start height
                fallStartHeight = 0;
            }
        }
    }




public float GetCurrentHealth() { return currentHealth; }
//prevent character from climbing slopes
void CalculateAndPrintSlopeAngle()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
           // Debug.Log("Slope Angle: " + slopeAngle);
        }
        else
        {
            Debug.LogWarning("No ground detected.");
        }
    }

void SlideDownSlope()
{
    RaycastHit hit;
    if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
    {
        float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        Debug.Log("Slope Angle: " + slopeAngle);
        Debug.DrawRay(hit.point, hit.normal, Color.red, 1f); // Debug visualization

        // Check if the slope angle is too steep
        if (slopeAngle > maxSlopeAngle)
        {
            // Calculate the slide direction aligned with gravity
            Vector3 gravityDirection = Physics.gravity.normalized;
            Vector3 slideDirection = Vector3.ProjectOnPlane(-gravityDirection, hit.normal).normalized;

            // Calculate the amount of slide based on the slope angle
            float slideAmount = Mathf.Lerp(0f, slideSpeed, Mathf.Clamp01((slopeAngle - maxSlopeAngle) / (90 - maxSlopeAngle))); // Adjusted to start sliding when slope exceeds maxSlopeAngle
            Debug.Log("Slide Amount: " + slideAmount);
            Debug.DrawRay(hit.point, slideDirection * slideAmount, Color.blue, 1f); // Debug visualization

            // Calculate the slide movement
            Vector3 slideMovement = slideDirection * slideAmount * Time.deltaTime;

            // Calculate new position along the slope
            Vector3 newPosition = transform.position + slideMovement;

            // Set the character's position directly
            transform.position = newPosition;
        }
    }
}
bool IsSlopeTooSteep()
{
    RaycastHit hit;
    if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
    {
        float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        return slopeAngle > maxSlopeAngle; // Return true if slope angle is too steep
    }
    return false;
}
}

