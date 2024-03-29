using UnityEngine;

public class ActivateMainCamera : MonoBehaviour
{
    void Start()
    {
        // Check if there is a main camera in the scene
        Camera mainCamera = Camera.main;
        
        if (mainCamera != null)
        {
            // Activate the main camera if it's not already active
            if (!mainCamera.gameObject.activeSelf)
            {
                mainCamera.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Main camera not found in the scene!");
        }
    }
}
