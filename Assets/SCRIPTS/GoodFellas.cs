using UnityEngine;

public class GoodFellas : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float followDistance = 5f; // Distance to follow the player
    public float stopDistanceFromPlayer = 1f; // Distance to stop moving when following the player
    public float enemyDetectionDistance = 5f; // Distance to detect enemies
    public float maxChaseDistance = 10f; // Maximum distance to chase an enemy
    public LayerMask enemyLayer; // LayerMask for enemies
    public float movementSpeed = 4f; // Adjustable moving speed

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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionDistance, enemyLayer);
        bool enemyDetected = colliders.Length > 0;
        Transform nearestEnemy = null;

        if (enemyDetected)
        {
            nearestEnemy = colliders[0].transform;
        }

        if (nearestEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, nearestEnemy.position);

            // Check if the distance to the enemy exceeds the maximum chase distance
            if (distanceToEnemy > maxChaseDistance)
            {
                // Give up chasing the enemy and return to the player
                currentTarget = player;
                isFollowingEnemy = false;
            }
            else
            {
                // If an enemy is detected within the follow distance, follow it
                if (distanceToEnemy <= followDistance)
                {
                    currentTarget = nearestEnemy;
                    isFollowingEnemy = true;
                }
                else
                {
                    // If the enemy is outside the follow distance, return to following the player
                    currentTarget = player;
                    isFollowingEnemy = false;
                }
            }
        }
        else
        {
            // If no enemy is detected, follow the player
            currentTarget = player;
            isFollowingEnemy = false;
        }

        // If the current target is the player or an enemy within the follow distance, continue following
        if (currentTarget != null)
        {
            if (currentTarget == player && distanceToPlayer <= stopDistanceFromPlayer)
            {
                // Stop moving if within stop distance from the player
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                MoveTowardsTarget(currentTarget);
                animator.SetFloat("Speed", 1f); // Adjust speed as needed
            }
        }
    }

    // Function to move towards the target
    void MoveTowardsTarget(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * movementSpeed);
    }

    // Draw Gizmos in the editor
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) // Only draw Gizmos in the editor, not during runtime
        {
            // Draw Gizmos for follow distance (blue)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, followDistance);

            // Draw Gizmos for stop distance from player (green)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, stopDistanceFromPlayer);

            // Draw Gizmos for enemy detection distance (red)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyDetectionDistance);

            // Draw Gizmos for max chase distance (yellow)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
        }
    }
}
