using UnityEngine;
using System.Collections; // Add this line to include the System.Collections namespace

public class FistColliderScript : MonoBehaviour
{
    public int damageAmount = 10;
    public float destroyDelay = 5.0f;
    public float kickbackForce = 100f; // Adjust as needed

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
                }
            }
            else if (other.CompareTag("piccolo"))
            {
                other.GetComponent<PiccoloHealthBar>().TakeDamage((float)damageAmount);

                hasHit = true;
                StartCoroutine(ResetHitStatus());
            }
              // else
                // {
                //     Debug.Log("Collider triggered but no valid target found.");
                // }
        }
    }

    IEnumerator ResetHitStatus()
    {
        yield return new WaitForSeconds(destroyDelay);
        hasHit = false;
    }
}
