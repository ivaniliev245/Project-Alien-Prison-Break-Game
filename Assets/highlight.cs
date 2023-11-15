using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class highlight : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;
    public float radius = 5f;
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer objectRenderer;
    public MonoBehaviour scriptToDisable;

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
    }

    private void Update()
    {
        // Check if the player is within the radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        bool playerInRange = false;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                playerInRange = true;

                // Change material if player is in range
                objectRenderer.material = highlightMaterial;

                // Press "F" to attach objectA
                if (Input.GetKeyDown(KeyCode.F))
                {
                    AttachObject(objectA);
                    objectRenderer.material = originalMaterial;
                }

                // Press "G" to attach objectB if it's within the radius
                if (Input.GetKeyDown(KeyCode.G))
                {
                    if (IsObjectBWithinRadius())
                    {
                        AttachObject(objectB);
                        objectRenderer.material = originalMaterial;
                        scriptToDisable.enabled = false;
                    }
                }
            }
        }

        // If player is not in range, revert to original material
        if (!playerInRange)
        {
            objectRenderer.material = originalMaterial;
        }
    }

    private void AttachObject(GameObject objToAttach)
    {
       //objToAttach.transform.parent = transform;
        //objToAttach.transform.position = transform.position;

        transform.parent = objToAttach.transform;
        transform.localPosition = Vector3.zero; // Optional: Set the local position relative to the parent
        transform.localRotation = Quaternion.identity; // Optional: Set the local rotation
    
    }

    private bool IsObjectBWithinRadius()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in colliders)
        {
            if (col.gameObject == objectB)
            {
                return true;
            }
        }
        return false;
    }
}