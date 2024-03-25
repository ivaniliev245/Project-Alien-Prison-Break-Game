using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAcidPool : MonoBehaviour
{
    public GameObject acidPool;
    public float spawnDistance = 1.5f;
    public float spawnCost = 20.0f;
    private float currentwater = 100.0f;
    public float cooldown = 10f;
    private float cooldownTimer;
    private bool isReady;
    

    // Start is called before the first frame update
    void Start()
    {
        currentwater = GetComponent<waterscript>().currentHealth;
        cooldownTimer = cooldown;
        isReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady)
        {
            cooldownTimer -= Time.deltaTime; 
            if (cooldownTimer <= 0f)
            {
                isReady = true;
                Debug.Log(isReady);
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && currentwater > 0 && isReady)
        {
            PlaceAcidPool();
            // Reduce current health in waterscript by the cost
            waterscript script = GetComponent<waterscript>();

            //set Ready Flag to false
            isReady = false;
            Debug.Log(isReady);
            if (script != null && script.currentHealth > 0)
            {
                script.Updatewaterbar(script.currentHealth - spawnCost);
            }
        }
        currentwater = GetComponent<waterscript>().currentHealth;
    }

    void PlaceAcidPool()
    {
        // Get the player's position
        Vector3 playerPosition = transform.position;

        // Calculate the position beneath the player
        Vector3 spawnPosition = new Vector3(playerPosition.x, playerPosition.y - spawnDistance, playerPosition.z);

        // Spawn the object at the calculated position
        Instantiate(acidPool, spawnPosition, Quaternion.identity);
    }

    
}