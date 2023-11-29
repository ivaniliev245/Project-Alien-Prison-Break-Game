using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodFellas : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float followDistance = 5f; // Distance to follow the player
    public float playerfollowDistance = 2f;
    public float enemyDetectionDistance = 5f; // Distance to detect enemies
    public LayerMask enemyLayer; // LayerMask for enemies

    private Transform currentTarget; // Current target to follow (player or enemy)

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
        }
    }

    // Function to make ObjectX follow the target
    void FollowTarget(Transform target)
    {
        // Move ObjectX towards the target
        if (currentTarget == player)
        {
              Vector3 modifiedTargetPosition = new Vector3(target.position.x + 2f, target.position.y, target.position.z);
              transform.position = Vector3.MoveTowards(transform.position, modifiedTargetPosition, Time.deltaTime * 3f);
        }
        else
        {
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 4f);
        }
    }
}