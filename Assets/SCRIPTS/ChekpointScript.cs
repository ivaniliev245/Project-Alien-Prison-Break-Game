using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CheckpointScript : MonoBehaviour
{
    public float detectionRange = 5f; // Adjustable range to detect player
    public Color startColor = Color.blue; // Initial emission color
    public Color endColor = Color.red; // Target emission color
    public GameObject checkpointPrefab; // Assignable prefab containing the emission material

    private Renderer rend; // Renderer component of the object
    private Renderer childRenderer; // Renderer component of the child object with the emission material
    private MaterialPropertyBlock propertyBlock; // Material Property Block to modify material properties

    private bool playerDetected = false; // Flag to track if player is detected

    void Start()
    {
        rend = GetComponent<Renderer>(); // Get the renderer component of the main object
        if (checkpointPrefab != null)
        {
            childRenderer = checkpointPrefab.GetComponentInChildren<Renderer>(); // Get the renderer component of the child object with the emission material

            if (childRenderer == null)
            {
                Debug.LogError("Renderer component not found on child object of the assigned prefab!");
                return;
            }
        }
        else
        {
            Debug.LogWarning("Checkpoint prefab not assigned!");
        }

        propertyBlock = new MaterialPropertyBlock(); // Create a new Material Property Block

        // Set the initial emission color
        SetEmissionColor(startColor);

        Debug.Log("Checkpoint script initialized.");
    }

    void Update()
    {
        // Check if player is within the detection range
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                // Player is within range, switch emission color if not already detected
                if (!playerDetected)
                {
                    SetEmissionColor(endColor);
                    playerDetected = true;
                    Debug.Log("Player detected. Emission color changed.");
                }
                return; // Exit the loop if player is found
            }
        }

        // Player is not within range, do nothing
    }

    // Set emission color for the material
    private void SetEmissionColor(Color color)
    {
        propertyBlock.SetColor("_EmissiveColor", color);
        childRenderer.SetPropertyBlock(propertyBlock);
    }

    // Draw gizmo to visualize detection range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
