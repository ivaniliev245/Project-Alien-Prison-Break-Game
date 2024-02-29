using System.Collections.Generic;
using UnityEngine;

public class LeaveSteps : MonoBehaviour
{
    public GameObject stepPrefab; // The prefab for the step object
    public float stepInterval = 0.5f; // Time interval between creating steps
    public float stepDistanceMultiplier = 1.0f; // Multiplier for step distance based on speed
    public Vector3 stepRotation = new Vector3(0f, 90f, 0f); // Rotation for the step
    public int maxStepCount = 20; // Maximum number of steps to keep

    public Vector3 leftStepOffset = new Vector3(-0.5f, 0f, 0f); // Offset for left step position
    public Vector3 rightStepOffset = new Vector3(0.5f, 0f, 0f); // Offset for right step position

    private Transform stepsParent;
    private List<GameObject> leftSteps = new List<GameObject>(); // List to hold instantiated left steps
    private List<GameObject> rightSteps = new List<GameObject>(); // List to hold instantiated right steps
    private Vector3 lastStepPosition; // Position of the last step

    public float speedThreshold = 0.2f;
    
    private void Start()
    {
        // Create a parent object for all steps
        stepsParent = new GameObject("StepsParent").transform;
    }


private void Update()
{
    // Move the character (you can replace this with your character movement logic)
    float horizontalInput = Input.GetAxis("Horizontal");
    float verticalInput = Input.GetAxis("Vertical");

    Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
    float speed = movement.magnitude;

    transform.Translate(movement * Time.deltaTime);

    // Check if it's time to leave a step based on distance traveled and minimum speed threshold
    if (speed > speedThreshold) // Adjust this threshold as needed
    {
        float distance = speed * stepDistanceMultiplier * Time.deltaTime;
        if (Vector3.Distance(transform.position, lastStepPosition) >= distance)
        {
            CreateStep();
            lastStepPosition = transform.position;
        }
    }
}

    private void CreateStep()
    {
        // Calculate the positions for the new steps with the offsets
        Vector3 leftStepPosition = transform.position + Quaternion.Euler(0, -90, 0) * leftStepOffset;
        Vector3 rightStepPosition = transform.position + Quaternion.Euler(0, 90, 0) * rightStepOffset;

        // Instantiate step prefabs at the calculated positions with rotation
        GameObject newLeftStep = Instantiate(stepPrefab, leftStepPosition, Quaternion.Euler(stepRotation), stepsParent);
        leftSteps.Add(newLeftStep);

        GameObject newRightStep = Instantiate(stepPrefab, rightStepPosition, Quaternion.Euler(stepRotation), stepsParent);
        rightSteps.Add(newRightStep);

        // Destroy the oldest steps if the number of steps exceeds the maximum count
        if (leftSteps.Count > maxStepCount || rightSteps.Count > maxStepCount)
        {
            DestroyOldestSteps();
        }
    }

    private void DestroyOldestSteps()
    {
        if (leftSteps.Count > 0)
        {
            GameObject oldestLeftStep = leftSteps[0];
            leftSteps.RemoveAt(0);
            Destroy(oldestLeftStep);
        }

        if (rightSteps.Count > 0)
        {
            GameObject oldestRightStep = rightSteps[0];
            rightSteps.RemoveAt(0);
            Destroy(oldestRightStep);
        }
    }
}