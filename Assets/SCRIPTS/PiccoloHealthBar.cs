using UnityEngine;
using UnityEngine.UI;

public class PiccoloHealthBar : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBar; // Reference to the UI Image component representing enemy health.
    public GameObject objectToHide;
    private float invincibilityLength;
    private float invincibilityCounter;

    void Start()
    {   
        objectToHide.SetActive(false);
        currentHealth = maxHealth;
        invincibilityLength = 0.25f;
        invincibilityCounter = invincibilityLength;
         // Hide the cursor at the beginning of the game
        Cursor.visible = false;

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        if (healthBar == null)
        {
            Debug.LogError("Health bar image is not assigned in the inspector!");
        }
    }

    public void Update() 
    {
        
        if (invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;
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

    public void TakeDamage(float damageAmount)
    {
        if (invincibilityCounter <= 0)
        {
            invincibilityCounter = invincibilityLength;
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
