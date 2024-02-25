using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public float timeBeforeSceneChange = 10f; // Adjustable time before scene change in seconds
    public string sceneToLoad; // Name of the scene to load, assign in the Unity Editor

    private float timer = 0f;
    private bool sceneChangeAllowed = false;

    void Update()
    {
        if (!sceneChangeAllowed)
        {
            timer += Time.deltaTime;
            if (timer >= timeBeforeSceneChange)
            {
                sceneChangeAllowed = true;
                LoadScene();
            }
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
