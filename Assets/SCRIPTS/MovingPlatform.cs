using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] waypoints; // Array of points the platform will move between
    public float speed = 2f; // Speed of the platform
    public GameObject objectToMove; // Object to move along with the platform (set in Inspector)

    private int currentWaypointIndex = 0;
    private Vector3 startPosition;
    private Vector3 nextPosition;
    public float delayTime = 2f;

    void Start()
    {
        startPosition = transform.position;
        nextPosition = waypoints[0].position;
    }

    void Update()
    {
        // Move the platform towards the next waypoint
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);

        // If the platform reaches the next waypoint, set the next waypoint as the target
        if (transform.position == nextPosition)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            nextPosition = waypoints[currentWaypointIndex].position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the one to move along with the platform
        if (other.gameObject == objectToMove)
        {
            other.transform.parent = this.transform; // Set the platform as the parent of the object
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the object is the one to move along with the platform and remove its parent
        if (other.gameObject == objectToMove)
        {
            other.transform.parent = null; // Remove the platform as the parent of the object
        }
    }
}
