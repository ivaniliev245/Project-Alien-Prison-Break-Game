using UnityEngine;
using UnityEngine.UI;

public class PiccoloHealthBar : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBar; // Reference to the UI Image component representing enemy health.
    public GameObject objectToHide;

    void Start()
    {   
        objectToHide.SetActive(false);
        currentHealth = maxHealth;

        if (healthBar == null)
        {
            Debug.LogError("Health bar image is not assigned in the inspector!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {  
        // Assuming the player has a collider tagged as "Attackhands"
        if (collision.collider.CompareTag("Attackhands"))
        {
            // Reduce enemy health
            TakeDamage(10f); // Adjust the amount of damage as needed
             Debug.Log("HIT!");
        }
    }

    void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // Ensure health doesn't go below 0
        currentHealth = Mathf.Max(0, currentHealth);

        // Update the UI health bar fill amount
        UpdateHealthBar();

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        // Calculate the fill amount based on current health
        float fillAmount = currentHealth / maxHealth;

        // Update the UI health bar fill amount
        if (healthBar != null)
        {
            healthBar.fillAmount = fillAmount;
        }
    }

    void Die()
    {
        // Perform any actions when the enemy is defeated
        Destroy(gameObject);
        objectToHide.SetActive(true);
    }
}