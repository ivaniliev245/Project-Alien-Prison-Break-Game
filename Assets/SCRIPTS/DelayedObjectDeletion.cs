using UnityEngine;

public class DelayedObjectDeletion : MonoBehaviour
{
    public float delay = 1f;

    private void Start()
    {
        // Invoke the deletion after the specified delay
        Invoke("DeleteObject", delay);
    }

    private void DeleteObject()
    {
        Debug.Log("Deleting object: " + gameObject.name);
        Destroy(gameObject);
    }
}
