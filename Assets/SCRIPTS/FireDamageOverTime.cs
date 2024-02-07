using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDamageOverTime : MonoBehaviour
{
    public int damagePerInterval = 10; // Damage to apply per interval
    public float damageInterval = 1f; // Interval between damage applications
    private Coroutine damageCoroutine; // Reference to the coroutine

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            PlatformerCharaterController playerController = collider.GetComponent<PlatformerCharaterController>();
            if (playerController != null)
            {
                // Start applying damage coroutine if not already running
                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(DealDamageOverTime(playerController));
                }
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            // Stop applying damage coroutine if player exits lava
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DealDamageOverTime(PlatformerCharaterController playerController)
    {
        while (true)
        {
            playerController.TakeDamage(damagePerInterval);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}