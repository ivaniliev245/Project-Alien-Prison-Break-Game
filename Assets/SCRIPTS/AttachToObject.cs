using UnityEngine;

public class AttachToObject : MonoBehaviour
{
    public Transform targetObject; // Drag the target object here in the Unity editor

    void Update()
    {
        if (targetObject != null)
        {
            // Match the position and rotation of the target object
            transform.position = targetObject.position;
            transform.rotation = targetObject.rotation;
        }
        else
        {
            Debug.LogWarning("Target object not assigned!");
        }
    }
}
