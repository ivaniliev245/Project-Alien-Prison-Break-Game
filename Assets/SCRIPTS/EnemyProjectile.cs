using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    // Adjust this value based on how much damage you want the projectile to deal
    public int damage = 10;

    private bool hasCollided = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is the player
        if (collision.gameObject.CompareTag("Player") && !hasCollided)
        {
            PlatformerCharaterController character = collision.gameObject.GetComponent<PlatformerCharaterController>();
            if (character != null)
            {
                character.TakeDamage(damage);
            }
            hasCollided = true;
            Destroy(gameObject);
        }
    }
}
