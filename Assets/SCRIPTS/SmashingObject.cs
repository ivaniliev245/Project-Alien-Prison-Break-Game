using UnityEngine;
using System.Collections; 

public class SmashingObject : MonoBehaviour
{
    public float smashSpeed = 5f;
    public float ascendSpeed = 2f;
    public float smashDelay = 2f;

    private Vector3 originalPosition;
    private bool isSmashing = false;

    void Start()
    {
        originalPosition = transform.position;
        InvokeRepeating("SmashToFloor", 0f, smashDelay);
    }

    void SmashToFloor()
    {
        if (!isSmashing)
        {
            isSmashing = true;
            StartCoroutine(SmashCoroutine());
        }
    }

    IEnumerator SmashCoroutine()
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = originalPosition - Vector3.up * 5f; // Adjust the downward distance

        while (elapsedTime < smashSpeed)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / smashSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // Ensure the object is precisely at the target position
        transform.position = targetPosition;

        yield return new WaitForSeconds(smashDelay);

        elapsedTime = 0f;

        while (elapsedTime < ascendSpeed)
        {
            transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / ascendSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object is precisely at the original position
        transform.position = originalPosition;

        isSmashing = false;
    }
}
