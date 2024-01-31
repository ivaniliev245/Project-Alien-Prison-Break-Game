using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LillyController : MonoBehaviour
{
    public GameObject objectToHide;

    // Assuming you have a reference to the enemy
    public bool enemydead = false;

    void Start()
    {
        // Hide the object at the beginning
        HideObject();
    }
    


    void Update()
    {
        // Check if the enemy is dead
        if (enemydead)
        {
            // If the enemy is dead, show the object
            ShowObject();
        }
        else
        {
            // If the enemy is alive or the reference is not set, hide the object
            HideObject();
        }
    }

    void ShowObject()
    {
        if (objectToHide != null)
        {
            objectToHide.SetActive(true);
        }
    }

    void HideObject()
    {
        if (objectToHide != null)
        {
            objectToHide.SetActive(false);
        }
    }
 }