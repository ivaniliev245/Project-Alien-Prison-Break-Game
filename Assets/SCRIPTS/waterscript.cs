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
    [SerializeField] private float animationDuration = 1.0f; // Duration of the animation

    public float currentHealth;

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
        if (currentHealth <= 0)
        {
            PlayerShoot script = GetComponent<PlayerShoot>();
            if (script != null)
            {
                script.Updatewater(script.currentwater - 1f);
            }
        }
        if (currentHealth > 0)
        {
            currentHealth -= amount;
            Updatewaterbar(currentHealth);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            StartCoroutine(TriggerAnimationAndDestroy(other.gameObject));
        }
    }

    private IEnumerator TriggerAnimationAndDestroy(GameObject obj)
    {
        // Trigger animation
        Animation anim = obj.GetComponent<Animation>();
        if (anim != null)
        {
            anim.Play(); // Play the animation
            yield return new WaitForSeconds(animationDuration);
        }

        // Destroy object after animationDuration
        Destroy(obj);

        // Collect health
        CollectHealth();
    }
    public void CollectWater(float amount)
    {
        this.currentHealth = currentHealth;
        currentHealth += amount;
        if (currentHealth > maxWater)
        {
            currentHealth = maxWater;
        }

        Updatewaterbar(currentHealth);
    }

    private void CollectHealth()
    {
        currentHealth += waterIncreaseAmount;
        if (currentHealth > maxWater)
        {
            currentHealth = maxWater;
        }

        Updatewaterbar(currentHealth);

        PlayerShoot script2 = GetComponent<PlayerShoot>();
        if (script2 != null)
        {
            script2.Updatewater(script2.currentwater + 1f);
        }
    }
}
