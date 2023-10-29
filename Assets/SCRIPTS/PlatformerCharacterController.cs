//Character player script

// values 

using UnityEngine;

public class PlatformerCharacterController : MonoBehaviour
{
     public float movementSpeed = 5f;
    public bool lockedRotation = true;
    public float lockedRotationY = 0f;
    // ... Other variables ...

    private CharacterController characterController;
    private Vector3 velocity;
    private bool grounded;
    private float jumpTimer;

    private Transform mainCamera;
    public float cameraFollowSpeed = 5.0f;
    public Vector3 cameraOffset = new Vector3(0, 2, -3);

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private Animator _animator;
    private const float _threshold = 0.01f;
    private bool _hasAnimator;

    public float gravity = -9.81f;
    public float jumpForce = 20f;
    public float jumpCooldown = 0.25f;
    void Start()
  {
        // Initialization code moved from the constructor
        characterController = GetComponent<CharacterController>();
        jumpTimer = 0f;
        mainCamera = Camera.main.transform;

        _animator = GetComponent<Animator>();
        _hasAnimator = _animator != null;
    }


void Update()
{
    // Calculate the player's movement input
    float horizontalInput = Input.GetAxis("Horizontal");
    float verticalInput = Input.GetAxis("Vertical");

    // Move the player based on their input
    Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

    // Apply movement only if not in the jump cooldown
 if (grounded && jumpTimer <= 0f)
        {
            movement = transform.TransformDirection(movement);
            movement.Normalize();
            velocity = movement * movementSpeed;

            // Set the "Speed" parameter in the Animator based on the character's speed
            if (_hasAnimator)
            {
                _animator.SetFloat("Speed", velocity.magnitude);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        grounded = characterController.isGrounded;
    
    
    
    // Apply gravity to the player
    velocity.y += gravity * Time.deltaTime;

    // CHECK IF THE PLAYER IS GROUNDED
    grounded = characterController.isGrounded;

    // Jump if the player presses the jump button and is grounded
    if (Input.GetButtonDown("Jump") && grounded && jumpTimer <= 0f)
    {
        velocity.y = jumpForce;
        jumpTimer = jumpCooldown;
    }

    // Limit the player's jump height
    if (velocity.y > jumpForce)
    {
        velocity.y = jumpForce;
    }

    // Decrease jump cooldown timer
    if (jumpTimer > 0f)
    {
        jumpTimer -= Time.deltaTime;
    }

    // Move the player
    characterController.Move(velocity * Time.deltaTime);

    
    
    
    // Update the player's rotation
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
            // Lock the player's rotation to the y-axis
            transform.rotation = Quaternion.Euler(0f, lockedRotationY, 0f);
        }
        else
        {
            // Allow the player to rotate freely
            transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X"), 0f));
        }
    }
}