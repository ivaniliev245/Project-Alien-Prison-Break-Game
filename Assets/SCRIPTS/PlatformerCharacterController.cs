using UnityEngine;

public class PlatformerCharacterController : MonoBehaviour
{
    // Adjustable character speed settings
    public float movementSpeed = 5f;

    // Locked rotation settings
    public bool lockedRotation = true;
    public float lockedRotationY = 0f;

    // Jump settings
    public float jumpForce = 20f;
    public float gravity = -9.81f;
    public float jumpCooldown = 0.25f;

    // Private variables
    private CharacterController characterController;
    private Vector3 velocity;
    private bool grounded;
    private float jumpTimer;

    private Transform mainCamera;
    public float cameraFollowSpeed = 5.0f;
    public Vector3 cameraOffset = new Vector3(0, 2, -3);


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        jumpTimer = 0f;
        mainCamera = Camera.main.transform;
   
   
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
        // Apply movement if there's input
        movement = transform.TransformDirection(movement); // Transform movement to local space
        movement.Normalize();
        velocity = movement * movementSpeed;
    }

    // Apply gravity to the player
    velocity.y += gravity * Time.deltaTime;

    // Check if the player is grounded
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