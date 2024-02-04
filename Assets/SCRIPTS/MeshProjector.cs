using UnityEngine;

[ExecuteInEditMode]
public class MeshProjector : MonoBehaviour
{
    public enum SnapDirection
    {
        Bottom,
        Top,
        All
    }

    public float maxDistance = 10f; // Maximum distance to search for objects
    public LayerMask objectLayerMask; // Layer mask to filter the objects
    public bool snapToMultipleObjects = false; // Whether to snap to multiple nearest objects
    public SnapDirection snapDirection = SnapDirection.Bottom; // Direction to snap
    public float volumeRetentionFactor = 0.5f; // Factor to control volume retention

    private Mesh originalMesh;
    private Mesh copiedMesh;

    void Start()
    {
        originalMesh = GetComponent<MeshFilter>().sharedMesh;
    }

    void Update()
    {
        if (originalMesh == null)
        {
            Debug.LogWarning("Original mesh not found. Make sure a MeshFilter component is attached with a valid mesh.");
            return;
        }

        if (copiedMesh == null)
        {
            copiedMesh = Instantiate(originalMesh); // Create a copy of the original mesh if not already copied
        }

        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, maxDistance, objectLayerMask);

        if (hits.Length == 0)
        {
            Debug.LogWarning("No objects found below the mesh projector within the max distance.");
            return;
        }

        foreach (RaycastHit hit in hits)
        {
            if (snapDirection == SnapDirection.Bottom && hit.normal != Vector3.up)
            {
                continue; // Skip this hit if snapping to bottom faces and the normal is not up
            }
            else if (snapDirection == SnapDirection.Top && hit.normal != Vector3.down)
            {
                continue; // Skip this hit if snapping to top faces and the normal is not down
            }

            ProjectMeshOntoSurface(hit);
            if (!snapToMultipleObjects)
            {
                break; // Exit the loop if snapping to a single object
            }
        }
    }

    void ProjectMeshOntoSurface(RaycastHit hit)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh sharedMesh = originalMesh; // Use the original mesh for projection

        Vector3[] sourceVertices = sharedMesh.vertices;
        Vector3[] projectedVertices = new Vector3[sourceVertices.Length];

        Vector3 surfaceNormal = hit.normal;
        Vector3 surfacePoint = hit.point;

        for (int i = 0; i < sourceVertices.Length; i++)
        {
            Vector3 vertexToProject = transform.TransformPoint(sourceVertices[i]); // Transform vertex to world space

            // Calculate the distance from the vertex to the surface plane
            float distanceToSurface = Vector3.Dot(vertexToProject - surfacePoint, surfaceNormal);

            // Adjust the projected position based on the distance to retain part of the volume
            Vector3 projectedPosition = vertexToProject - surfaceNormal * Mathf.Min(distanceToSurface, volumeRetentionFactor);

            // Transform the projected position back to local space of the source mesh
            Vector3 localProjectedPosition = transform.InverseTransformPoint(projectedPosition);

            // Store the projected vertex position
            projectedVertices[i] = localProjectedPosition;
        }

        copiedMesh.vertices = projectedVertices; // Update the copied mesh vertices
        copiedMesh.RecalculateBounds(); // Recalculate bounds for rendering

        meshFilter.mesh = copiedMesh; // Assign the copied mesh back to the mesh filter
    }
}
