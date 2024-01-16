using UnityEngine;

public class CorrectInputDisplay : MonoBehaviour
{
    public float displayDuration = 3.0f;
    public Color newEmissionColor = Color.green; // Set the desired emission color
    public float newEmissionIntensity = 1.0f; // Set the desired emission intensity

    private Material originalMaterial;
    private Color originalEmissionColor;
    private float originalEmissionIntensity;

    private void Start()
    {
        // Store the original material, emission color, and emission intensity at the start
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
            originalEmissionColor = originalMaterial.GetColor("_EmissiveColor");
            originalEmissionIntensity = originalMaterial.GetFloat("_EmissiveIntensity");
        }
        else
        {
            Debug.LogError("Renderer component not found on the object.");
        }
    }

    // Call this method to display the correct input object
    public void DisplayCorrectInputObject()
    {
        ChangeObjectEmission();
        Invoke("ResetObject", displayDuration);
    }

    private void ChangeObjectEmission()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Change the emission color and intensity
            originalMaterial.SetColor("_EmissiveColor", newEmissionColor);
            originalMaterial.SetFloat("_EmissiveIntensity", newEmissionIntensity);
        }
        else
        {
            Debug.LogError("Renderer component not found on the object.");
        }
    }

    private void ResetObject()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Reset the emission color and intensity to their original states
            originalMaterial.SetColor("_EmissiveColor", originalEmissionColor);
            originalMaterial.SetFloat("_EmissiveIntensity", originalEmissionIntensity);
        }
        else
        {
            Debug.LogError("Renderer component not found on the object.");
        }
    }
}
