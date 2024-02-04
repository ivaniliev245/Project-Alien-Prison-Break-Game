using UnityEngine;

[ExecuteInEditMode]
public class MeshProjector : MonoBehaviour
{
    public float maxDistance = 10f; // Maximum distance to search for objects
    public LayerMask objectLayerMask; // Layer mask to filter the objects
    public bool snapToMultipleObjects = false; // Whether to snap to multiple nearest objects

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

            // Project the vertex onto the surface plane
            Vector3 projectedPosition = vertexToProject - Vector3.Dot(vertexToProject - surfacePoint, surfaceNormal) * surfaceNormal;

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
