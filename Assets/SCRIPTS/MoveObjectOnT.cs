using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectOnT : MonoBehaviour
{
    public GameObject objectX; // Assign your object X in the Inspector
    public float moveDistance = 0.02f; // 20 millimeters = 0.02 meters

    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.T))
        {
            MoveObjectX();
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
        objectX.transform.Translate(Vector3.up * moveDistance);
    }
}