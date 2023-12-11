using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float moveRange = 5f; // Set the range the enemy can move
    public float moveSpeed = 2f; // Set the speed of movement
    private float initialPositionX;
    private bool movingRight = true;

    private Transform player;
    private bool isChasing = false;
    //private bool isRoaming = true;
    public float detectionRange = 10.0f;


    void Start()
    {
        initialPositionX = transform.position.x; // Store the initial position of the enemy
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > detectionRange)
        {
            isChasing = false;
        }



        if (!isChasing)
        {
        // Move the enemy back and forth within the given range
        if (movingRight)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

        // Check if the enemy reaches the range limit, then change direction
        if (Mathf.Abs(transform.position.x - initialPositionX) >= moveRange)
        {
            movingRight = !movingRight;
        }

        }


        if (isChasing)
        {
            
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            
        }





    }
}
