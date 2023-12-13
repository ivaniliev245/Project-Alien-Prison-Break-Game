using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectOnT : MonoBehaviour
{
    //public GameObject objectX; // Assign your object X in the Inspector
    public float moveDistance = 0.02f; // 20 millimeters = 0.02 meters
    public float moveSpeed = 5f;
    private bool isPlayerNearby = false;
    private bool dooropened = false;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.T))
        {
            if (!dooropened)
        {
            MoveObjectX();
            dooropened = true;
            Debug.Log("DOOR OPENED");
        }
            else if (dooropened)
        {
            MoveObjectXDown();
            dooropened = false;
            Debug.Log("DOOR CLOSED");
        }
          
        }
         
      
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    void MoveObjectX()
    {
        // Move the object X upwards
        transform.Translate(Vector3.up * moveDistance);
    }
    void MoveObjectXDown()
    {
        // Move the object X upwards
         transform.Translate(Vector3.down * moveDistance);
    }
}