using UnityEngine;
using Cinemachine;

public class CameraFocalLengthControl : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float newFocalLength = 30f; // Set the desired new focal length
    public float damping = 5f; // Set the desired damping value

    private float originalFocalLength;
    private float currentFocalLength;

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine virtual camera not assigned!");
            return;
        }

        // Store the original focal length
        originalFocalLength = virtualCamera.m_Lens.FieldOfView;
        currentFocalLength = originalFocalLength;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Set the new focal length
            StartCoroutine(SmoothTransition(newFocalLength));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Revert to the original focal length
            StartCoroutine(SmoothTransition(originalFocalLength));
        }
    }

    private System.Collections.IEnumerator SmoothTransition(float targetFocalLength)
    {
        while (!Mathf.Approximately(currentFocalLength, targetFocalLength))
        {
            // Smoothly interpolate towards the target focal length
            currentFocalLength = Mathf.Lerp(currentFocalLength, targetFocalLength, Time.deltaTime * damping);

            // Apply the new focal length
            virtualCamera.m_Lens.FieldOfView = currentFocalLength;

            yield return null;
        }
    }
}
