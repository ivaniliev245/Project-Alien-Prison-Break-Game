using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPick : MonoBehaviour
{
 public int damage = 10; // Amount of damage the rock deals
 private PlatformerCharaterController playerController;
 public GameObject player;

    void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the one to move along with the platform
        if (other.gameObject == player)
        {
           
            playerController = other.GetComponent<PlatformerCharaterController>();
            if (playerController != null)
            {
               //damageCoroutine = StartCoroutine(DealDamageOverTime());
               playerController.TakeDamage(damage);
               //Destroy(gameObject);
            }
        }
    }
}

