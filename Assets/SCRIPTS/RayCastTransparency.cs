using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class RayCastTransperency : MonoBehaviour
{
    public Transform target; // The target object
    public float fadeStartDistance = 5f; // Distance to start fading
    public float fadeEndDistance = 10f; // Distance to complete fading

    private void Update()
    {
        if (target == null)
        {
            Debug.LogError("Target object not assigned to the script.");
            return;
        }

        // Calculate the distance from the camera to the target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Calculate the transparency based on the distance
        float transparency = Mathf.InverseLerp(fadeStartDistance, fadeEndDistance, distanceToTarget);

        // Clamp the transparency value between 0 and 1
        transparency = Mathf.Clamp01(transparency);

        // Apply transparency to all HDRP materials between the camera and the target
        ApplyTransparency(transparency);
    }

    private void ApplyTransparency(float transparency)
    {
        RaycastHit[] hits;

        // Cast a ray from the camera to the target
        Vector3 direction = target.position - transform.position;
        Ray ray = new Ray(transform.position, direction);
        hits = Physics.RaycastAll(ray, direction.magnitude);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Check if the renderer uses HDRP's HighDefinitionRenderPipelineMaterial
                var hdrpMaterial = renderer.material as Material;
                if (hdrpMaterial != null && hdrpMaterial.HasProperty("_SurfaceType"))
                {
                    // Assuming you have a "_Dissolve" property in your HDRP shader for controlling transparency
                    hdrpMaterial.SetFloat("_Dissolve", transparency);
                }
            }
        }
    }
}