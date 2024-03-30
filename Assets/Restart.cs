using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public string sceneName; // Name of the scene to load (you can set this in the Inspector)
    public string sceneName2;

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName); // Load the specified scene by name
        //Debug.Log("Restart!");
    }

    public void LoadScene2()
    {
        SceneManager.LoadScene(sceneName2); // Load the specified scene by name
    }
}
