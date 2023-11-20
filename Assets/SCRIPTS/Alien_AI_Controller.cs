using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien_AI_Controller : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public float moveSpeed = 3f; // Speed at which the enemy moves towards the player

    private Rigidbody enemyRigidbody;

    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>(); // Getting the Rigidbody component of the enemy
    }

    void Update()
    {
        if (player != null)
        {
            // Calculate the direction from the enemy to the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Move the enemy towards the player
            enemyRigidbody.MovePosition(transform.position + directionToPlayer * moveSpeed * Time.deltaTime);

            // Rotate the enemy to face the player (optional)
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            enemyRigidbody.MoveRotation(lookRotation);
        }
    }
}