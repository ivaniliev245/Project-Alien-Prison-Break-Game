using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Healthbar : MonoBehaviour
{
    [SerializeField] private Slider healthbar;
    private Camera camRef;

    public void Start()
    {
        camRef = Camera.main;
    }

    public void UpdateHealthbar(float currentHealth, float maxHealth)
    {
        healthbar.value = currentHealth / maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - camRef.transform.position);
    }
}
