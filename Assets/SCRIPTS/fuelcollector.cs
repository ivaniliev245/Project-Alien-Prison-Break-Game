using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fuelcollector : MonoBehaviour
{
    public Material newMaterial; // Assign the material you want to apply in the Inspector
    public Material oldmaterial;
    public Renderer playerRenderer;
    public Endgame Endgame;

   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lavalilly"))
        {
            Debug.Log("Collectable collected!");
            ChangeMaterial();
            Destroy(other.gameObject);
            SetCondition(true);
             //Time.timeScale = 0f;
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
        }
    }

    void ChangeMaterial()
    {
        //Renderer playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null && newMaterial != null)
        {
            playerRenderer.material = newMaterial;
            Debug.Log("Material changed!");
        }
        else
        {
            Debug.LogError("Player Renderer or New Material is null. Check the Inspector settings.");
        }
    }

     void SetCondition(bool newCondition)
    {
        // Set the condition in the ColliderController script
        Endgame.fuelfilled = newCondition;

        // Log a message (you can remove this in the final version)
        Debug.Log($"Condition set to {newCondition}");
    }
}
