using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CheckpointScript : MonoBehaviour
{
    public float detectionRange = 5f;
    public Color startColor = Color.blue;
    public Color endColor = Color.red;
    public GameObject checkpointPrefab;
    public GameObject emissionObject; // Reference to the child GameObject with emission material
    public AudioClip detectionSound;
    public float vfxDuration = 2f;
    public GameObject vfxPrefab;
    public Transform vfxSpawnPoint;
    public List<Light> lightComponents; // List of assignable HDRP Light components

    private Renderer rend;
    private Renderer childRenderer;
    private Material childMaterial; // Material of the child object with emission
    private MaterialPropertyBlock propertyBlock;
    private AudioSource audioSource;
    private bool playerDetected = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        if (checkpointPrefab != null)
        {
            childRenderer = checkpointPrefab.GetComponentInChildren<Renderer>();

            if (childRenderer == null)
            {
                Debug.LogError("Renderer component not found on child object of the assigned prefab!");
                return;
            }
            // Get the material of the child object
            childMaterial = childRenderer.material;
        }
        else
        {
            Debug.LogWarning("Checkpoint prefab not assigned!");
        }

        propertyBlock = new MaterialPropertyBlock();

        // Set the initial emission color
        SetEmissionColor(startColor);

        Debug.Log("Checkpoint script initialized.");
    }

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                if (!playerDetected)
                {
                    SetEmissionColor(endColor);
                    playerDetected = true;
                    Debug.Log("Player detected. Emission color changed.");

                    if (audioSource != null && detectionSound != null)
                    {
                        audioSource.PlayOneShot(detectionSound);
                    }

                    if (vfxPrefab != null && vfxSpawnPoint != null)
                    {
                        GameObject vfxInstance = Instantiate(vfxPrefab, vfxSpawnPoint.position, vfxSpawnPoint.rotation);
                        Destroy(vfxInstance, vfxDuration);
                    }
                }
                return;
            }
        }
    }

    private void SetEmissionColor(Color color)
    {
        propertyBlock.SetColor("_EmissiveColor", color);
        childRenderer.SetPropertyBlock(propertyBlock);

        foreach (Light lightComponent in lightComponents)
        {
            if (lightComponent != null)
            {
                // Change the HDRP light's color directly
                lightComponent.color = color;
            }
        }

        // Change the emission color of the child object's material
        if (childMaterial != null)
        {
            childMaterial.SetColor("_EmissiveColor", color);
        }

        // Change the emission color of the specific child GameObject's material
        if (emissionObject != null)
        {
            Renderer emissionRenderer = emissionObject.GetComponent<Renderer>();
            if (emissionRenderer != null)
            {
                Material emissionMaterial = emissionRenderer.material;
                emissionMaterial.SetColor("_EmissiveColor", color);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
