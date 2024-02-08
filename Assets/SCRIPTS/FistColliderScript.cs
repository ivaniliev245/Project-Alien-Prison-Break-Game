using UnityEngine;

public class FistColliderScript : MonoBehaviour
{
    public int damageAmount = 10;
    public float destroyDelay = 5.0f;
    public float kickbackForce = 100f; // Adjust as needed

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            Alien_AI_Controller enemyController = other.GetComponent<Alien_AI_Controller>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(damageAmount);
                
                // Apply kickback force to the enemy
                Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();
                if (enemyRigidbody != null)
                {
                    Vector3 kickbackDirection = (other.transform.position - transform.position).normalized;
                    enemyRigidbody.AddForce(kickbackDirection * kickbackForce, ForceMode.Impulse);
                }
            }

            // Destroy the collider after hitting the enemy
            Destroy(gameObject);
        }
        else if (other.CompareTag("piccolo"))
        {
            other.GetComponent<PiccoloHealthBar>().TakeDamage((float)damageAmount);
            Destroy(gameObject);
        }
    }
}
