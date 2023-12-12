using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Healthbar : MonoBehaviour
{
    [SerializeField] private Slider healthbar;
    [SerializeField] private Vector3 offset;
    public Camera camRef;

    public void Start()
    {
        camRef = Camera.main;
    }

    public void UpdateHealthbar(float currentHealth, float maxHealth)
    {
        healthbar.value = currentHealth / maxHealth;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - camRef.transform.position);

    }
}
