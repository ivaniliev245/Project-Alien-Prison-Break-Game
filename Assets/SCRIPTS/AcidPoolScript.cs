using System.Collections;
using UnityEngine;

public class AcidPoolScript : MonoBehaviour
{
    public int damageAmount = 10;
    public float destroyDelay = 5.0f; // Time before the acid pool disappears
    public float animationDelay = 1.0f; // Time before playing the die animation
    public Animation acidAttackAnimation; // Reference to the Animation component

    private bool hasCollided = false; // Flag to track if the acid has collided with an object

    void Start()
    {
        acidAttackAnimation = GetComponent<Animation>(); // Assuming the Animation component is attached to the same GameObject
        StartCoroutine(DestroyAfterDelay());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Alien_AI_Controller>().TakeDamage(damageAmount);
            hasCollided = true;
        }
        else if (other.CompareTag("piccolo"))
        {
            other.GetComponent<PiccoloHealthBar>().TakeDamage(damageAmount);
            hasCollided = true;
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);

        if (hasCollided && acidAttackAnimation != null)
        {
            acidAttackAnimation.Play("acid_attack_splash_die");
            yield return new WaitForSeconds(animationDelay);

            // Wait for the animation to finish
            while (acidAttackAnimation.isPlaying)
            {
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
