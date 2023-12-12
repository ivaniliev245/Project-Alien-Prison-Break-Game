using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PlatformerCharaterController : MonoBehaviour
{
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
    private float newLifeHeight = 0.3f;// For Crouching
    private Vector3 originalLifePosition;// For Crouching
    private Vector3 newLifePosition;// For Crouching
    private Vector3 oldCenter;// For Crouching
    private Vector3 velocity;
    private bool grounded;
    private float jumpTimer;
    public float coyoteTime;
    public float jumpInputTimeBuffer;

    //float timers
    private float lastOnGroundTime;
    private float lastPressedJumpTime;
    
    private bool isJumpFalling;

    private Transform mainCamera;
    public float cameraFollowSpeed = 5.0f;
    public Vector3 cameraOffset = new Vector3(0, 2, -3);
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
    //private bool isRunning = false;

    // Add the jump animation parameter
    private bool isJumping = false;
    private bool IsSliding; //check for wall sliding
    private bool DJumping = false;
    //crouch parameter
    private bool isCrouching = false;
    private float crouchD = -1.1f;
    //attack parameters
    private bool isAttacking = false;
    private string[] attackAnimations = { "AttackR", "AttackL", "AttackL_v2", "AttackR_v2", "StrongAttack" };
    
    // handle crouching 
    private const string crouchNoWalkParam = "crouchNoWalk";  
  
    // Update Rotation
    public float followAxis = 0.0f;
   
    //handle jump duration
    public float jumpDuration = 0.5f;
   
   
    void Start()
    {
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        jumpTimer = 0f;
        mainCamera = Camera.main.transform;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        hasAnimator = animator != null;
        originalLifePosition = Life.transform.position;
    }

    void Update()
    {
        lastOnGroundTime -= Time.deltaTime; 
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool runInput = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed = runInput ? runSpeed : walkSpeed;
        float currentAnimationSpeedMultiplier = runInput ? runAnimationSpeedMultiplier : walkAnimationSpeedMultiplier;

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        bool isMoving = movement.magnitude > 0.1f; // Check if the character is moving
        bool isCrouchingNoWalk = !isMoving && isCrouching; // Not moving and crouching
        bool isCrouchingWithMove = isMoving && isCrouching; // Moving and crouching
        
        bool isWalkingAttack = isMoving && isAttacking;

    if (isWalkingAttack)
    {
        // Set the parameter in the animator to trigger the walking attack animation
        if (hasAnimator)
        {
            animator.SetBool("AttackWhileWalking", true);
        }
    }
    else
    {
        // Reset the parameter in the animator if not performing a walking attack
        if (hasAnimator)
        {
            animator.SetBool("AttackWhileWalking", false);
        }
    }
        if (grounded && jumpTimer <= 0f)
        {
            movement = transform.TransformDirection(movement);
            movement.Normalize();
            velocity = movement * currentSpeed;

            if (hasAnimator)
            {
                if (grounded) // Only update speed if grounded
                {
                    animator.SetFloat("Speed", velocity.magnitude *
                         currentAnimationSpeedMultiplier, locomotionSmoothTime, Time.deltaTime);
                    isJumping = false;
                }
                
                animator.SetBool("crouchNoWalk", isCrouchingNoWalk);
                animator.SetBool("Crouch", isCrouchingWithMove);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleCrouch();
                Debug.Log("Splash is crouching");
            }

            if (Input.GetKeyUp(KeyCode.C))
            {
                ToggleCrouchUp();
                Debug.Log("Splash is not crouching");
                animator.SetBool("Crouch", false);
            }
            if (Input.GetMouseButtonDown(0)) // 0 represents left mouse button, change as needed
            {
                StartAttack(); // Call method to initiate attack
            }

            if (Input.GetMouseButtonUp(0)) // 0 represents left mouse button, change as needed
            {
                StopAttack(); // Call method to stop attack
            }
        
        }

        velocity.y += gravity * Time.deltaTime;
        
        
        //character is grouned  -- removed from reap from you... to be depricated
        grounded = characterController.isGrounded;
        
        if (characterController.isGrounded)
        {
            lastOnGroundTime = coyoteTime;
        }
        Debug.Log(lastOnGroundTime);

        characterController.Move(velocity * Time.deltaTime);

        UpdateRotation();
        HandleJump();
        UpdateGroundedState();
       
        if (mainCamera != null)
        {
            Vector3 cameraTargetPosition = transform.position + cameraOffset;
            mainCamera.position = Vector3.Lerp(mainCamera.position, cameraTargetPosition, Time.deltaTime * cameraFollowSpeed);
        }
    }

 private void UpdateLife()
    {
            float movementSpeed = 20.0f;
            float step = movementSpeed * Time.deltaTime;
            Vector3 newPosition  = new Vector3(transform.position.x, originalLifePosition.y, transform.position.z);
            Life.transform.position = Vector3.Lerp(Life.transform.position, newPosition, step);
           
            //Life.SetActive(true);
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
            childObject.rotation = Quaternion.Slerp(childObject.rotation, newRotation, Time.deltaTime * rotationSpeed);
        }
    }
}

   void ToggleCrouch()
    {
          if (!isCrouching)
            {   
                // Set the trigger parameter in the animator to start the animation
                animator.SetBool("Crouch", true);
              
                isCrouching = true;
                //Life.SetActive(false);
                
                oldHeight = characterController.height;
                // Life.transform.position = new Vector3(transform.position.x, transform.position.y + crouchD, transform.position.z);
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
        animator.SetBool("Crouch", false);
        isCrouching = false;
        characterController.center = oldCenter;
        characterController.height = oldHeight;
        // UpdateLife();   
    } 
   }
void StartAttack()
{
    isAttacking = true;

    // Get a random attack animation name from the array
    string randomAttack = attackAnimations[Random.Range(0, attackAnimations.Length)];

    // Trigger the randomly selected attack animation immediately without completing the idle animation cycle
    if (hasAnimator)
    {
        animator.CrossFade(randomAttack, 0); // 0 indicates the transition should be instant
    }
}

void StopAttack()
{
    isAttacking = false;
    // Reset attack-related flags or animations here
    // For example:
    // If you don't need to stop a specific attack animation by name, this might not be necessary

    // Reset the parameter in the animator when the attack stops
    if (hasAnimator)
    {
        animator.SetBool("AttackWhileWalking", false);
    }
}


void HandleJump()
{
    if (grounded && jumpTimer <= 0f)
    {
        if (!isJumping && Input.GetButtonDown("Jump"))
        {
            StartCoroutine(ApplyJumpForce());
        }
    }

    // Apply gravity to the character
    velocity.y += gravity * Time.deltaTime;

    // Ensure the character doesn't exceed the jump force
    if (velocity.y > jumpForce)
    {
        velocity.y = jumpForce;
    }

    // Move the character using CharacterController
    characterController.Move(velocity * Time.deltaTime);
}

IEnumerator ApplyJumpForce()
{
    float timeInAir = 0f;
    float initialJumpForce = Mathf.Sqrt(jumpForce * -2f * gravity);
    isJumping = true;
    
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

    isJumping = false;
    //important value for animator pls dont reomove
    if (hasAnimator)
    {
        animator.SetBool("Jump", false);
    }
}


void UpdateGroundedState()  // this is only for animator
{
    if (grounded && jumpTimer <= 0f)
    {
        // Check if the character is touching the ground
        bool isCharacterGrounded = characterController.isGrounded;

        // Set the 'Grounded' boolean parameter in the animator based on character's grounded state
        if (hasAnimator)
        {
            animator.SetBool("Grounded", isCharacterGrounded);
        }
    }
    else
    {
        // If not grounded or jump timer active, set the 'Grounded' parameter to false
        //important value for animator pls dont remove
        
        if (hasAnimator)
        {
            animator.SetBool("Grounded", false);
        }
    }
}






}