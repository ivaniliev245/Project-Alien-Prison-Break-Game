using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class waterscript : MonoBehaviour
{
    [SerializeField] private Slider waterbar;
    [SerializeField] private float maxWater = 100f;
    [SerializeField] private float waterDecreaseRate = 1f;
    [SerializeField] private float waterIncreaseAmount = 20f;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxWater;
        StartCoroutine(DecreaseHealthOverTime());
    }

    private IEnumerator DecreaseHealthOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Adjust this time interval as needed
            DecreaseHealth(waterDecreaseRate);
        }
    }

    public void Updatewaterbar(float currentHealth)
    {
        this.currentHealth = currentHealth;
        waterbar.value = currentHealth / maxWater;
    }

    private void DecreaseHealth(float amount)
    {
        if (currentHealth > 0)
        {
            currentHealth -= amount;
            Updatewaterbar(currentHealth);

            // Check if health is zero or less, you might want to handle player death here
            if (currentHealth <= 0)
            {
                // Handle player death, reset level, etc.
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Water");
        if (other.CompareTag("Water"))
        {
            Debug.Log("Water");
            CollectHealth();
            Destroy(other.gameObject); // Assuming you want to destroy the collectable
        }
    }

    private void CollectHealth()
    {
        currentHealth += waterIncreaseAmount;
        if (currentHealth > maxWater)
        {
            currentHealth = maxWater;
        }

        Updatewaterbar(currentHealth);
    }
}
