using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerKinematic : MonoBehaviour
{
    public Rigidbody objectC; // Reference to Object C's Rigidbody
    private Vector3 initialPosition; // To store initial position

    
    private void Start()
    {
   
    initialPosition = transform.position; // Save initial position of Object C
    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Replace "YourTag" with the tag of the triggering object
        {
            StartCoroutine(SetKinematicAfterDelay());
            //Debug.Log("Start");
        }
    }

    private System.Collections.IEnumerator SetKinematicAfterDelay()
    {
        yield return new WaitForSeconds(1.0f); // Wait for seconds

        if (objectC != null)
        {
            objectC.isKinematic = false; // Set Object C's Rigidbody to be kinematic
        }
        else
        {
            Debug.LogWarning("Object C's Rigidbody reference is not set!");
        }
    }
        public void ResetStats()
    {
        transform.position = initialPosition;
        objectC.isKinematic = true;
    }
}
