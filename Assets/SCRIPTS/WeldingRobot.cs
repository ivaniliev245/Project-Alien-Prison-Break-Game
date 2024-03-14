using UnityEngine;

public class WeldingRobot : MonoBehaviour
{
    public Transform[] waypoints; // Waypoints to define the path of the robot
    public float speed = 2f; // Speed of the robot
    public float stoppingDistance = 0.1f; // Distance at which the robot stops at a waypoint
    public Animator animator; // Animator for the welding animation
    public GameObject weldingVFX; // VFX effect for welding

    private int currentWaypointIndex = 0; // Index of the current waypoint
    private bool isWelding = false; // Flag to check if welding animation is playing

    void Update()
    {
        // Move the robot towards the current waypoint
        if (!isWelding && waypoints.Length > 0)
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Check if the robot has reached the waypoint
            if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Move to the next waypoint
            }
        }
    }

    // Function to start welding at the current position
    public void StartWelding()
    {
        if (!isWelding)
        {
            isWelding = true;
            animator.SetTrigger("StartWelding"); // Trigger the welding animation
            Instantiate(weldingVFX, transform.position, Quaternion.identity); // Spawn welding VFX effect
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
