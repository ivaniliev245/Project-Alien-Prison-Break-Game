using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacle_rotate_z : MonoBehaviour
{

    public float rotationSpeed = 50f; // Adjust the speed as needed
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        // Rotate the object around its X-axis based on the rotationSpeed
        //transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
