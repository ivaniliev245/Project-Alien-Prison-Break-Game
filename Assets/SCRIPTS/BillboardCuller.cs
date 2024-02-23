using UnityEngine;

public class BillboardCuller : MonoBehaviour
{
    public float showDistance = 10f;
    public float cullDistance = 20f;
    public GameObject objectToCull;

    private Transform objectTransform;
    private Camera mainCamera;

    private void Start()
    {
        objectTransform = objectToCull.transform;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (objectToCull == null || mainCamera == null)
            return;

        float distanceToCamera = Vector3.Distance(objectTransform.position, mainCamera.transform.position);

        if (distanceToCamera >= showDistance && distanceToCamera <= cullDistance)
        {
            // Object is within show distance and cull distance, make it visible
            objectToCull.SetActive(true);
        }
        else
        {
            // Object is outside show distance and cull distance, hide it
            objectToCull.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, showDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cullDistance);
    }
}
