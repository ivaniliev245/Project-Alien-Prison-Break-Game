using UnityEngine;
using UnityEngine.UI;
public class HealthbarEnemy : MonoBehaviour
{
    [SerializeField] private Slider healthbar;
    private Vector3 offset;
    private Camera camRef;
    public float rotationSpeed = 5f;
    private Quaternion targetRotation;

    public void Start()
    {
        camRef = Camera.main;
    }

    public void UpdateHealthbar(float currentHealth, float maxHealth)
    {
        healthbar.value = currentHealth / maxHealth;
    }

    void LateUpdate()
    {
        // Calculate the target rotation based on the camera's orientation
        targetRotation = Quaternion.LookRotation(camRef.transform.forward, camRef.transform.up);

        // Smoothly rotate the health bar towards the target rotation
        healthbar.transform.rotation = Quaternion.Slerp(healthbar.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
