using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAttachment : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;
    public GameObject thirdObject;
    public GameObject fourthObject;
    public GameObject fifthObject;
    public Animator fifthObjectAnimator;
    public string animationTrigger = "YourAnimationTriggerName";

    public Material attachedMaterial;
    public Material defaultMaterial;

    private bool objectsAttached = false;

    private void Update()
    {
        // Checking if objects A and B are attached
        if (!objectsAttached && objectA.transform.parent == objectB.transform && objectA.transform.parent != null)
        {
            // Objects A and B are attached
            ChangeThirdObjectMaterial();
            DestroyFourthObject();
            PlayFifthObjectAnimation();
            objectsAttached = true;
        }
        else if (objectsAttached && (objectA.transform.parent != objectB.transform || objectA.transform.parent == null))
        {
            // Objects A and B are detached
            RestoreThirdObjectMaterial();
            objectsAttached = false;
        }
    }

    private void ChangeThirdObjectMaterial()
    {
        if (thirdObject != null && attachedMaterial != null)
        {
            Renderer renderer = thirdObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = attachedMaterial;
            }
        }
    }

    private void RestoreThirdObjectMaterial()
    {
        if (thirdObject != null && defaultMaterial != null)
        {
            Renderer renderer = thirdObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = defaultMaterial;
            }
        }
    }

    private void DestroyFourthObject()
    {
        if (fourthObject != null)
        {
            Destroy(fourthObject);
        }
    }

    private void PlayFifthObjectAnimation()
    {
        if (fifthObjectAnimator != null)
        {
            fifthObjectAnimator.SetTrigger(animationTrigger);
        }
    }
}