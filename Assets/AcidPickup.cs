using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPickup : MonoBehaviour
{
    [SerializeField] private float waterAmount = 20f; // Amount of water to increase when collected

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the waterscript component from the player
            waterscript playerWaterscript = other.GetComponent<waterscript>();

            if (playerWaterscript != null)
            {
                // Increase the player's water content
                playerWaterscript.CollectWater(waterAmount);

                // Destroy the collectible object
                Destroy(gameObject);
            }
        }
    }
}
