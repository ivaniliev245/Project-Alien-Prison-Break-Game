using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endgame : MonoBehaviour
{
 
    // Add your condition variable here
    public bool fuelfilled = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player (you might need to adjust the tag or layer)
        if (other.CompareTag("Player"))
        {
            // Check your specific condition
            if (fuelfilled)
            {
                // Stop the game
                Time.timeScale = 0f; // This freezes the game

                // Optionally, you can put other game stop logic here

                // Log a message (you can remove this in the final version)
                Debug.Log("Game Stopped!");
            }
        }
    }
}
