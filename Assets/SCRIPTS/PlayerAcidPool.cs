using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import the UI namespace

public class PlayerAcidPool : MonoBehaviour
{
    public GameObject acidPool; // The acid pool GameObject to be instantiated
    public float spawnDistance = 1.5f; // Distance from the player where the acid pool will spawn
    public float spawnCost = 20.0f; // Cost in water for spawning an acid pool
    private float currentwater = 100.0f; // Player's current water supply
    public float cooldown = 10f; // Cooldown time in seconds
    private float cooldownTimer; // Timer to track the cooldown
    private bool isReady; // Flag to check if the player can place an acid pool
    public GameObject cooldownUI; // UI element to show the cooldown status

    // Start is called before the first frame update
    void Start()
    {
        // Initialize current water and cooldown timer
        currentwater = GetComponent<waterscript>().currentHealth;
        cooldownTimer = 0f;
        isReady = true;
        cooldownUI.SetActive(false); // Ensure the cooldown UI is initially inactive
    }

    // Update is called once per frame
    void Update()
    {
        // Update the cooldown timer if the player is not ready to place another acid pool
        if (!isReady)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isReady = true;
                cooldownUI.SetActive(false); // Deactivate cooldown UI when ready
                Debug.Log("Ready to place acid pool");
            }
        }

        // Check if the player presses the "R" key, has enough water, and is ready to place an acid pool
        if (Input.GetKeyDown(KeyCode.R) && currentwater >= spawnCost && isReady)
        {
            PlaceAcidPool();
            
            // Reduce current water in waterscript by the cost
            waterscript script = GetComponent<waterscript>();

            // Set the ready flag to false and reset the cooldown timer
            isReady = false;
            cooldownTimer = cooldown;
            cooldownUI.SetActive(true); // Activate cooldown UI
            Debug.Log("Acid pool placed, cooldown started");

            // Update the water bar if the script is available
            if (script != null && script.currentHealth > 0)
            {
                script.Updatewaterbar(script.currentHealth - spawnCost);
            }
        }

        // Update the current water value from the waterscript
        currentwater = GetComponent<waterscript>().currentHealth;
    }

    void PlaceAcidPool()
    {
        // Get the player's position
        Vector3 playerPosition = transform.position;

        // Calculate the position beneath the player
        Vector3 spawnPosition = new Vector3(playerPosition.x, playerPosition.y - spawnDistance, playerPosition.z);

        // Spawn the acid pool at the calculated position
        Instantiate(acidPool, spawnPosition, Quaternion.identity);
    }
}
