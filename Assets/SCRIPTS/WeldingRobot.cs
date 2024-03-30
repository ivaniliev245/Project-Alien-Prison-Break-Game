using UnityEngine;

public class WeldingRobot : MonoBehaviour
{
    public Transform[] waypoints; // Waypoints to define the path of the robot
    public float speed = 2f; // Speed of the robot
    public float stoppingDistance = 0.1f; // Distance at which the robot stops at a waypoint
    public float rotationSpeed = 5f; // Speed of rotation
    public Animator animator; // Animator for the welding animation
    public GameObject weldingVFXPrefab; // Prefab for welding VFX effect
    public float weldingVFXTimer = 1f; // Duration of welding VFX effect
    public float waypointStayDuration = 1f; // Duration to stay at each waypoint
    public Transform vfxSpawnPoint; // Spawn point for welding VFX effect

    private int currentWaypointIndex = 0; // Index of the current waypoint
    private bool isWelding = false; // Flag to check if welding animation is playing
    private bool isWaitingAtWaypoint = false; // Flag to check if the robot is waiting at a waypoint
    private float waitTimer = 0f; // Timer to track the duration of waiting at a waypoint

    void Update()
    {
        if (!isWelding && waypoints.Length > 0)
        {
            if (isWaitingAtWaypoint)
            {
                // If waiting at a waypoint, decrement the timer
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    isWaitingAtWaypoint = false;
                    MoveToNextWaypoint();
                }
            }
            else
            {
                // Move the robot towards the current waypoint
                Vector3 targetPosition = waypoints[currentWaypointIndex].position;
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                // Rotate the robot towards the movement direction
                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                // Check if the robot has reached the waypoint
                if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
                {
                    // Start waiting at the waypoint
                    isWaitingAtWaypoint = true;
                    waitTimer = waypointStayDuration;

                    // Spawn welding VFX effect at the specified spawn point
                    if (weldingVFXPrefab != null && vfxSpawnPoint != null)
                    {
                        GameObject weldingVFX = Instantiate(weldingVFXPrefab, vfxSpawnPoint.position, Quaternion.identity);
                        Destroy(weldingVFX, weldingVFXTimer); // Destroy the VFX effect after a certain duration
                    }
                }
            }
        }
    }

    // Function to move to the next waypoint
    private void MoveToNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Move to the next waypoint
    }

    // Function to start welding at the current position
    public void StartWelding()
    {
        if (!isWelding)
        {
            isWelding = true;
            animator.SetTrigger("StartWelding"); // Trigger the welding animation
        }
    }

    // Function to stop welding
    public void StopWelding()
    {
        if (isWelding)
        {
            isWelding = false;
            animator.SetTrigger("StopWelding"); // Trigger the stop welding animation
        }
    }
}
