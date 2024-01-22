using UnityEngine;
using Cinemachine;

public class CameraControlScreen : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float newScreenYValue = 0.5f; // Set the desired new screen Y value
    public float damping = 5f; // Set the desired damping value

    private float originalScreenYValue;
    private float currentScreenYValue;

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine virtual camera not assigned!");
            return;
        }

        // Store the original screen Y value
        originalScreenYValue = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY;
        currentScreenYValue = originalScreenYValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Set the new screen Y value
            StartCoroutine(SmoothTransition(newScreenYValue));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Revert to the original screen Y value
            StartCoroutine(SmoothTransition(originalScreenYValue));
        }
    }

    private System.Collections.IEnumerator SmoothTransition(float targetScreenY)
    {
        while (!Mathf.Approximately(currentScreenYValue, targetScreenY))
        {
            // Smoothly interpolate towards the target screen Y value
            currentScreenYValue = Mathf.Lerp(currentScreenYValue, targetScreenY, Time.deltaTime * damping);

            // Apply the new screen Y value
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = currentScreenYValue;

            yield return null;
        }
    }
}
