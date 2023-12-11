using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitch : MonoBehaviour
{
    public KeyCode switchKey = KeyCode.Tab; // Change this to your desired key
    public float switchDistance = 3f; // Change this to the distance for switching
    public GameObject mainCamera; // Reference to the main camera

    private GameObject[] players;
    private GameObject currentPlayer;
    private bool isSwitched = false;

    void Start()
    {
        // Find all player GameObjects in the scene
        players = GameObject.FindGameObjectsWithTag("Player");

        // Set the default player as the starting player
        currentPlayer = players[0]; // Change this to set your default player

        // Find and assign the main camera if not set in the Inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main.gameObject;
        }
    }

    void Update()
    {
        // Check if the switch key is pressed
        if (Input.GetKeyDown(switchKey))
        {
            // Find the closest player within the switch distance
            GameObject closestPlayer = FindClosestPlayer();

            // Switch to the closest player if found and not already switched
            if (closestPlayer != null && !isSwitched)
            {
                currentPlayer = closestPlayer;
                isSwitched = true;

                // Update the camera's target to the new player
                UpdateCameraTarget(currentPlayer.transform);
            }
            // If already switched, revert to default player
            else if (isSwitched)
            {
                currentPlayer = players[0]; // Change this to set your default player
                isSwitched = false;

                // Update the camera's target back to the default player
                UpdateCameraTarget(currentPlayer.transform);
            }
        }
    }

    GameObject FindClosestPlayer()
    {
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(player.transform.position, currentPosition);
            Debug.Log("DISTANCE: "+distance);
            Debug.Log("P: "+players.Length);

            if (distance < closestDistance && distance < switchDistance && player != currentPlayer)
            {
                closestDistance = distance;
                closest = player;
            }
        }

        return closest;
    }

    void UpdateCameraTarget(Transform newTarget)
    {
        // Set the camera's target to follow the new player
        mainCamera.GetComponent<CameraFollow>().SetTarget(newTarget);
    }
}
