using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodFellas : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float followDistance = 5f; // Distance to follow the player
    public float enemyDetectionDistance = 5f; // Distance to detect enemies
    public LayerMask enemyLayer; // LayerMask for enemies

    private Transform currentTarget; // Current target to follow (player or enemy)
    private Animator animator; // Reference to the Animator component

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        // Get the Animator component attached to the child object
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Calculate distance between ObjectX and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if an enemy is within the detection range
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionDistance, enemyLayer);
        bool enemyDetected = colliders.Length > 0;

        // If an enemy is detected, set it as the target
        if (enemyDetected)
        {
            currentTarget = colliders[0].transform;
        }
        else
        {
            currentTarget = player;
        }

        // If the current target is not null and it's within the follow distance, follow it
        if (currentTarget != null && distanceToPlayer <= followDistance)
        {
            FollowTarget(currentTarget);

            // Calculate speed based on distance to the target
            float speed = (currentTarget == player) ? 1f : 0.5f; // Adjust as needed

            // Update the Animator speed parameter
            animator.SetFloat("Speed", speed);
        }
        else
        {
            // If not following, set speed to 0 for idle animation
            animator.SetFloat("Speed", 0f);
        }
    }

    // Function to make ObjectX follow the target
    void FollowTarget(Transform target)
    {
        // Rotate towards the target
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        // Move ObjectX towards the target
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 4f);
    }
}
