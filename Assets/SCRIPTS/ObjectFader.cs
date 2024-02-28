using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    
    public float fadeSpeed, fadeAmount;
    float originalOpacity;

    public new MeshRenderer renderer;
    Material Mat;
    public bool DoFade = false;


    
    // Start is called before the first frame update
    void Start()
    {
        Mat = GetComponent <MeshRenderer>().material;
        originalOpacity = Mat.color.a;

    }

    // Update is called once per frame
    void Update()
    {
        if(DoFade)
            FadeNow();
        else 
            ResetFade();
        
    }

    void FadeNow()
    {
        Color currentColor = Mat.color;
        Color smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b, 
            Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));
        Mat.color = smoothColor;   
    }

    public void ResetFade()
{
    Color currentColor = Mat.color;

    // Check if the alpha value is close to the original opacity before updating
    if (!Mathf.Approximately(currentColor.a, originalOpacity))
    {
        Color smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b,
            Mathf.Lerp(currentColor.a, originalOpacity, fadeSpeed * Time.deltaTime));

        // Ensure alpha does not go below the original opacity
        smoothColor.a = Mathf.Min(smoothColor.a, originalOpacity);

        Mat.color = smoothColor;
    }
    else
    {
        DoFade = false; // Stop resetting once the alpha is back to the original opacity
    }
}

}

