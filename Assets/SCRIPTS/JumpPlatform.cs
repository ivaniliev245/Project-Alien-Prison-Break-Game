using UnityEngine;

public class SpringPlatform : MonoBehaviour
{
    public float springForce = 10f; // Adjust this to set the force applied by the spring
    public float delayTime = 2f; // Time to wait before launching the object

    private bool canLaunch = true;
    private Rigidbody targetRigidbody;

    private void OnCollisionEnter(Collision collision)
    {
        if (canLaunch && collision.gameObject.CompareTag("Player")) // Assuming the player has the "Player" tag
        {
            targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (targetRigidbody != null)
            {
                canLaunch = false;
                Invoke("LaunchObject", delayTime);
            }
        }
    }

    private void LaunchObject()
    {
        if (targetRigidbody != null)
        {
            targetRigidbody.AddForce(Vector3.up * springForce, ForceMode.Impulse);
        }
        canLaunch = true;
    }
}
