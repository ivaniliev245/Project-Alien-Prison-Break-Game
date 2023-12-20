using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitch : MonoBehaviour
{
    public KeyCode switchKey = KeyCode.Tab; // Change this to your desired key
    public float switchDistance = 3f; // Change this to the distance for switching
    public GameObject mainCamera; // Reference to the main camera
    public GameObject mainplayer;
    private Animator animator;
    

    private GameObject[] players;
    public GameObject currentPlayer;
    private bool isSwitched = false;
    private GameObject currentEnemy = null;


    void Start()
    {   
        animator = GetComponentInChildren<Animator>();
        // Find all player GameObjects in the scene
        players = GameObject.FindGameObjectsWithTag("Enemy");

        // Set the default player as the starting player
        currentPlayer = players[0]; // Change this to set your default player

        // Find and assign the main camera if not set in the Inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main.gameObject;
        }
    }

    void Update()
    {   freeze freeze = mainplayer.GetComponent<freeze>();
        
        

        // Check if the switch key is pressed
        if (Input.GetKeyDown(switchKey) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            // Find the closest player within the switch distance
            GameObject closestPlayer = null;
            closestPlayer = FindClosestPlayer();

            // Switch to the closest player if found and not already switched
           if (isSwitched)
            {   if(currentEnemy != null)
                {
                    EnemyBehaviour eb = currentEnemy.GetComponent<EnemyBehaviour>();
                    EnemyBehaviour2 eb2 = currentEnemy.GetComponent<EnemyBehaviour2>();
                    eb.enabled=true;
                    eb2.enabled=false;
                    currentEnemy= null;
                }
                currentPlayer = mainplayer; // Change this to set your default player
                isSwitched = false;           
                animator.SetBool("Meditate", false);  
                freeze.Unfreeze();              
                // Update the camera's target back to the default player
                UpdateCameraTarget(currentPlayer.transform);
                Debug.Log("OLD PLAYER");
            }
        
            else if (closestPlayer != null && !isSwitched)
            {   
                currentPlayer = closestPlayer;
                EnemyBehaviour eb = currentPlayer.GetComponent<EnemyBehaviour>();
                EnemyBehaviour2 eb2 = currentPlayer.GetComponent<EnemyBehaviour2>();
                eb.enabled=false;
               eb2.enabled=true;
               currentEnemy =  currentPlayer;
                isSwitched = true;                
                Debug.Log("NEW PLAYER");
                Debug.Log("Current Player " + currentPlayer);
                freeze.Freeze();
                animator.SetBool("Meditate", true);
                // Update the camera's target to the new player
                UpdateCameraTarget(currentPlayer.transform);
            }
            // If already switched, revert to default player

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


            if (distance < closestDistance && distance < switchDistance && player != mainplayer)
            {
                closestDistance = distance;
                closest = player;
               // Debug.Log("DISTANCE: "+distance  + player.name);
               // Debug.Log("P: "+players.Length);
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
