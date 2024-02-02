using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAcidPool : MonoBehaviour
{
    public GameObject acidPool;
    public float spawnDistance = 1.5f;
    public float spawnCost = 2.0f;
    private float currentwater = 100.0f;
    

    // Start is called before the first frame update
    void Start()
    {
        currentwater = GetComponent<waterscript>().currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentwater > 0)
        {
            PlaceAcidPool();
            // Reduce current health in waterscript by 2
            waterscript script = GetComponent<waterscript>();
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