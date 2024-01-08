using System.Collections.Generic;
using UnityEngine;

public class LeaveSteps : MonoBehaviour
{
    public GameObject stepPrefab; // The prefab for the step object
    public float stepInterval = 0.5f; // Time interval between creating steps
    public float stepDistanceMultiplier = 1.0f; // Multiplier for step distance based on speed
    public Vector3 stepOffset = new Vector3(0f, 0f, -0.5f); // Offset for step position
    public Vector3 stepRotation = new Vector3(0f, 90f, 0f); // Rotation for the step
    private float stepTimer = 0f; // Timer to control step creation

    private List<GameObject> steps = new List<GameObject>(); // List to hold instantiated steps
    private Vector3 lastStepPosition; // Position of the last step

    private void Update()
    {
        // Move the character (you can replace this with your character movement logic)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        float speed = movement.magnitude;

        transform.Translate(movement * Time.deltaTime);

        // Check if it's time to leave a step based on distance traveled
        if (speed > 0)
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
        // Calculate the position for the new step with the offset
        Vector3 stepPosition = transform.position + stepOffset;

        // Instantiate a step prefab at the calculated position
        GameObject newStep = Instantiate(stepPrefab, stepPosition, Quaternion.Euler(stepRotation));
        steps.Add(newStep);

        // You can modify the step's properties here (e.g., size, rotation, etc.)

        // Destroy the oldest step if the number of steps exceeds a certain limit
        if (steps.Count > 20) // Change the limit as needed
        {
            DestroyOldestStep();
        }
    }

    private void DestroyOldestStep()
    {
        if (steps.Count > 0)
        {
            GameObject oldestStep = steps[0];
            steps.RemoveAt(0);
            Destroy(oldestStep);
        }
    }
}