using UnityEngine;
using System.Collections;

public class FistColliderScript : MonoBehaviour
{
    public int damageAmount = 10;
    public float destroyDelay = 5.0f;
    public float kickbackForce = 100f;
    public GameObject hitVFXPrefab; // Reference to the VFX prefab
    public float vfxDuration = 2.0f; // Duration for which the VFX sticks to the enemy

    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit)
        {
            if (other.CompareTag("Enemy"))
            {
                Alien_AI_Controller enemyController = other.GetComponent<Alien_AI_Controller>();
                Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();

                if (enemyController != null && enemyRigidbody != null)
                {
                    enemyController.TakeDamage(damageAmount);
                    Vector3 kickbackDirection = (other.transform.position - transform.position).normalized;
                    enemyRigidbody.AddForce(kickbackDirection * kickbackForce, ForceMode.Impulse);

                    hasHit = true;
                    StartCoroutine(ResetHitStatus());

                    // Instantiate hit VFX at the collision point
                    GameObject hitVFX = Instantiate(hitVFXPrefab, other.transform.position, Quaternion.identity);
                    // Make the VFX stick to the enemy
                    hitVFX.transform.parent = other.transform;
                    // Remove the VFX after vfxDuration seconds
                    Destroy(hitVFX, vfxDuration);
                }
            }
                else if (other.CompareTag("piccolo"))
                {
                    PiccoloHealthBar piccoloHealth = other.GetComponent<PiccoloHealthBar>();
                    if (piccoloHealth != null)
                    {
                        piccoloHealth.TakeDamage((float)damageAmount);
                        // Instantiate hit VFX at the collision point
                        GameObject hitVFX = Instantiate(hitVFXPrefab, other.transform.position, Quaternion.identity);
                        if (hitVFX != null)
                        {
                            // Make the VFX stick to Piccolo
                            hitVFX.transform.parent = other.transform;
                            // Remove the VFX after vfxDuration seconds
                            Destroy(hitVFX, vfxDuration);
                        }
                    }
                }


    IEnumerator ResetHitStatus()
    {
        yield return new WaitForSeconds(destroyDelay);
        hasHit = false;
    }
}}}
