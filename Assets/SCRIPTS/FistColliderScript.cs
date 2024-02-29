using UnityEngine;
using System.Collections;

public class FistColliderScript : MonoBehaviour
{
    public int damageAmount = 10;
    public float destroyDelay = 5.0f;
    public float kickbackForce = 100f;
    public GameObject hitVFXPrefab; // Reference to the VFX prefab
    public float vfxDuration = 2.0f; // Duration for which the VFX sticks to the enemy
    public float minScale = 0.5f; // Minimum scale for the VFX
    public float maxScale = 1.5f; // Maximum scale for the VFX

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
                    // Randomize the scale of the VFX
                    float randomScale = Random.Range(minScale, maxScale);
                    hitVFX.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                    // Make the VFX stick to the enemy
                    hitVFX.transform.parent = other.transform;
                    // Remove the VFX after vfxDuration seconds
                    Destroy(hitVFX, vfxDuration);
                }
            }
            else if (other.CompareTag("piccolo"))
            {
                other.GetComponent<PiccoloHealthBar>().TakeDamage((float)damageAmount);

                hasHit = true;
                StartCoroutine(ResetHitStatus());

                // Instantiate hit VFX at the collision point
                GameObject hitVFX = Instantiate(hitVFXPrefab, other.transform.position, Quaternion.identity);
                // Randomize the scale of the VFX
                float randomScale = Random.Range(minScale, maxScale);
                hitVFX.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
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
}
