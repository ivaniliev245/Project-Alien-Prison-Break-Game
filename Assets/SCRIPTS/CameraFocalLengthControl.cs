using UnityEngine;
using Cinemachine;

public class CameraFOVControl : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float newFOV = 80f; // Set the desired new field of view
    public float transitionDuration = 1f; // Set the duration of the transition

    private float originalFOV;
    private bool isTransitioning = false;

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine virtual camera not assigned!");
            return;
        }

        // Store the original field of view
        originalFOV = virtualCamera.m_Lens.FieldOfView;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            // Set the new field of view with a smooth transition
            StartCoroutine(SmoothTransition(newFOV));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            // Revert to the original field of view with a smooth transition
            StartCoroutine(SmoothTransition(originalFOV));
        }
    }

    private System.Collections.IEnumerator SmoothTransition(float targetFOV)
    {
        isTransitioning = true;

        float elapsed_time = 0f;
        float initialFOV = virtualCamera.m_Lens.FieldOfView;

        while (elapsed_time < transitionDuration)
        {
            // Smoothly interpolate towards the target field of view
            float newFOVValue = Mathf.Lerp(initialFOV, targetFOV, elapsed_time / transitionDuration);
            virtualCamera.m_Lens.FieldOfView = newFOVValue;

            elapsed_time += Time.deltaTime;
            yield return null;
        }

        // Ensure the field of view is set to the exact target value
        virtualCamera.m_Lens.FieldOfView = targetFOV;
        isTransitioning = false;
    }
}
