using UnityEngine;

public class ToggleGameObjectAndCursor : MonoBehaviour
{
    public GameObject pauseScreen;

    void Start()
    {
        // Ensure pause screen is initially inactive
        if (pauseScreen != null)
        {
            pauseScreen.SetActive(false);
        }
    }

    void Update()
    {
        // Check for Escape key press to toggle pause screen
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseScreen();
        }
    }

    void TogglePauseScreen()
    {
        // Toggle the pause screen visibility
        if (pauseScreen != null)
        {
            bool isPaused = !pauseScreen.activeSelf;
            pauseScreen.SetActive(isPaused);

            // Lock or unlock cursor depending on pause state
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isPaused;
        }
    }

    // Optional: Function to resume the game
    public void ResumeGame()
    {
        TogglePauseScreen();
    }
}
