using System.Collections;
using System.Collections.Generic;
using Unity.FPS.UI;
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
        player.GetComponent<PlatformerCharaterController>().SetCurrentHealth(100);
        float currentHealth = player.GetComponent<PlatformerCharaterController>().GetCurrentHealth();
        float maxHealth = player.GetComponent<PlatformerCharaterController>().maxHealth;
        player.GetComponent<HealthbarPlayer>().UpdateHealthbar(currentHealth,maxHealth);
    }
}
