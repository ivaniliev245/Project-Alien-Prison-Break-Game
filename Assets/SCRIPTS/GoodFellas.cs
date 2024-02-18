using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodFellas : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float followDistance = 5f; // Distance to follow the player
    public float stopDistanceFromPlayer = 1f; // Distance to stop moving when following the player
    public float enemyDetectionDistance = 5f; // Distance to detect enemies
    public LayerMask enemyLayer; // LayerMask for enemies

    private Transform currentTarget; // Current target to follow (player or enemy)
    private Animator animator; // Reference to the Animator component
    private bool isFollowingEnemy; // Flag to indicate if currently following an enemy

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        // Get the Animator component attached to the child object
        animator = GetComponentInChildren<Animator>();

        // Initialize flags
        isFollowingEnemy = false;
    }

    void Update()
    {
        // Calculate distance between ObjectX and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if an enemy is within the detection range
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionDistance, enemyLayer);
        bool enemyDetected = colliders.Length > 0;
        Transform nearestEnemy = null;

        // If an enemy is detected, set it as the target
        if (enemyDetected)
        {
            nearestEnemy = colliders[0].transform;
        }

        // If the current target is not null and it's within the follow distance, follow it
        if (nearestEnemy != null && distanceToPlayer <= followDistance)
        {
            currentTarget = nearestEnemy;
            isFollowingEnemy = true;
        }
        else
        {
            currentTarget = player;
            isFollowingEnemy = false;
        }

        // If the current target is the player and the distance is less than or equal to the follow distance
        if (currentTarget == player && distanceToPlayer <= followDistance)
        {
            // If the character is within the stop distance from the player, stop moving
            if (distanceToPlayer <= stopDistanceFromPlayer)
            {
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                // Otherwise, continue following the player
                FollowTarget(player);
                animator.SetFloat("Speed", 1f); // Adjust speed as needed
            }
        }
        else if (currentTarget != null)
        {
            // Continue following the enemy without stopping
            FollowTarget(currentTarget);
            animator.SetFloat("Speed", 1f); // Adjust speed as needed
        }
    }

    // Function to make ObjectX follow the target
    void FollowTarget(Transform target)
    {
        // Rotate towards the target
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        // Move ObjectX towards the target
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 4f);
    }
}
