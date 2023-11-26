using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneTrigger : MonoBehaviour
{
    public Transform respawnLocation; // Assign the respawn location in the Unity Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnPlayer(other.gameObject);
        }
    }

    private void RespawnPlayer(GameObject player)
    {
        player.transform.position = respawnLocation.position;
        // You might want to add more logic here like resetting player stats, etc.
    }
}
