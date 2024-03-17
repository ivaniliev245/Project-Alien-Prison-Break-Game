using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacle_rotate : MonoBehaviour
{
   public float rotationSpeed = 50f; // Adjust the speed as needed

    // Update is called once per frame
    void Update()
    {
        // Rotate the object around its Y-axis based on the rotationSpeed
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
