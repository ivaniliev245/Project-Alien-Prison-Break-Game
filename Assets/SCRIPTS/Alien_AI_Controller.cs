using UnityEngine;

public class Alien_AI_Controller : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public float moveSpeed = 2f; // Speed at which the enemy moves towards the player
    public float rotationSpeed = 2f; // Speed at which the enemy rotates towards the player

    private Rigidbody enemyRigidbody;

    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>(); // Getting the Rigidbody component of the enemy

        // Lock rotation along X and Z axes
        enemyRigidbody.freezeRotation = true;
    }

    void Update()
    {
        if (player != null)
        {
            // Calculate the direction from the enemy to the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Move the enemy towards the player gradually
            Vector3 targetPosition = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            enemyRigidbody.MovePosition(targetPosition);

            // Rotate the enemy to face the player smoothly
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
