using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarPlayer : MonoBehaviour
{
    [SerializeField] private Slider healthbar;
    public void UpdateHealthbar(float currentHealth, float maxHealth)
    {
        healthbar.value = currentHealth / maxHealth;
    }
}
