using UnityEngine;
using Cinemachine;

public class CameraHorizontalMovement : MonoBehaviour
{
    public float speed = 1f; // Speed of camera movement
    public float offsetX = 10f; // Offset from the default screen X value of 0.5
    public float holdDurationThreshold = 0.5f; // Minimum duration to hold A or D keys to initiate movement
    public float minXOffset = -10f; // Minimum offset from default screen X value
    public float maxXOffset = 10f; // Maximum offset from default screen X value
    public float centerDelay = 1f; // Delay before starting centering process (adjustable)
    public float centeringDuration = 1f; // Time to return to center if no input is detected
    public float damping = 5f; // Custom damping value for smoother transitions

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer framingTransposer;
    private bool isHoldingKey = false;
    private float holdStartTime;
    private bool isCentering = false;
    private float centeringStartTime;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider is the trigger collider that activates the camera movement
        if (other.CompareTag("CameraMovementTrigger"))
        {
            enabled = true; // Enable the camera movement script
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the collider is the trigger collider that deactivates the camera movement
        if (other.CompareTag("CameraMovementTrigger"))
        {
            enabled = false; // Disable the camera movement script
        }
    }

    void Update()
    {
        if (!enabled) return; // Exit the update loop if the script is disabled

        // Check if A or D keys are held down
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            // If not already holding key, record start time
            if (!isHoldingKey)
            {
                holdStartTime = Time.time;
                isHoldingKey = true;
                isCentering = false; // Stop centering if player has input
            }
            else
            {
                // Check if hold duration exceeds threshold
                if (Time.time - holdStartTime >= holdDurationThreshold)
                {
                    // Apply offset based on input
                    float inputHorizontal = Input.GetKey(KeyCode.D) ? -1f : 1f;
                    float defaultScreenX = 0.5f;
                    float targetX = Mathf.Clamp(framingTransposer.m_ScreenX + inputHorizontal * speed * Time.deltaTime, 
                                                 defaultScreenX + minXOffset, 
                                                 defaultScreenX + maxXOffset);
                    framingTransposer.m_ScreenX = targetX;
                }
            }
        }
        else
        {
            isHoldingKey = false; // Reset holding key flag when keys are released
        }

        // Start centering process after delay
        if (!isHoldingKey && !isCentering && Time.time - holdStartTime >= centerDelay)
        {
            isCentering = true;
            centeringStartTime = Time.time;
        }

        // Smoothly return the camera to the center
        if (isCentering)
        {
            float elapsedTime = Time.time - centeringStartTime;
            float t = Mathf.Clamp01(elapsedTime / centeringDuration);
            float currentX = framingTransposer.m_ScreenX;
            float targetX = Mathf.SmoothStep(currentX, 0.5f, t);
            framingTransposer.m_ScreenX = targetX;

            if (t >= 1f)
            {
                isCentering = false;
            }
        }
    }
}
