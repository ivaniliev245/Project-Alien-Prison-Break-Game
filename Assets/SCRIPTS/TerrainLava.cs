using System.Collections;
using UnityEngine;

public class TerrainLava : MonoBehaviour
{
    public int normalDamagePerInterval = 10; // Normal damage to apply per interval
    public int fireDamagePerInterval = 20; // Fire damage to apply per interval while player is on fire
    public float damageInterval = 1f; // Interval between damage applications
    public GameObject vfxPrefab; // Prefab with visual effects
    public Transform vfxSpawnPoint; // Transform where second VFX prefab spawns
    public GameObject secondVfxPrefab; // Second prefab with visual effects
    public float onFireDuration = 5f; // Duration the player remains on fire

    private PlatformerCharaterController playerController; // Reference to the player controller
    private bool isPlayerOnFire = false; // Flag to track if player is on fire
    private GameObject currentFirstVfx; // Reference to the instantiated first VFX
    private GameObject currentSecondVfx; // Reference to the instantiated second VFX

    private Coroutine damageCoroutine; // Reference to the coroutine

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlatformerCharaterController>();
            if (playerController != null && !isPlayerOnFire)
            {
                // Set player on fire and start applying damage coroutine
                isPlayerOnFire = true;
                damageCoroutine = StartCoroutine(DealDamageOverTime());
                // Spawn first visual effects if available
                if (vfxPrefab != null && vfxSpawnPoint != null)
                {
                    currentFirstVfx = Instantiate(vfxPrefab, vfxSpawnPoint.position, Quaternion.identity);
                }
                // Spawn second visual effects if player is on fire
                if (secondVfxPrefab != null && vfxSpawnPoint != null)
                {
                    currentSecondVfx = Instantiate(secondVfxPrefab, vfxSpawnPoint.position, Quaternion.identity);
                    // Set the parent of the second VFX to the vfxSpawnPoint to follow its position
                    currentSecondVfx.transform.parent = vfxSpawnPoint;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && isPlayerOnFire)
        {
            // Apply fire damage if player is in contact with lava and on fire
            playerController.TakeDamage(fireDamagePerInterval);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop applying damage coroutine if player exits lava
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
            isPlayerOnFire = false; // Reset the flag when player exits lava
            playerController = null; // Reset playerController reference
            
            // Optionally, remove visual effects here
            if (currentFirstVfx != null)
            {
                Destroy(currentFirstVfx);
            }
            if (currentSecondVfx != null)
            {
                Destroy(currentSecondVfx);
            }
        }
    }

    private IEnumerator DealDamageOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
