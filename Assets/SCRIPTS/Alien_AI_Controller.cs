using UnityEngine;

public class Alien_AI_Controller : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public float moveSpeed = 2f; // Speed at which the enemy moves towards the player
    public float rotationSpeed = 2f; // Speed at which the enemy rotates towards the player

    private Rigidbody enemyRigidbody;

    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>(); // Getting the Rigidbody component of the enemy

<<<<<<< HEAD
        // Lock rotation along X and Z axes
        enemyRigidbody.freezeRotation = true;
    }

    void Update()
    {
        if (player != null)
=======
    //States
    public float sightRange, attackRange;
    bool playerInSightRange, playerInAttackRange;

    //Health
    public float maxhealth;
    private float currentHealth;

    [SerializeField] private Healthbar healthbar;

    void Update()
    {
        //Check if Player is in sight- and AttackRange
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);


        if (playerInSightRange && playerInAttackRange) { AttackPlayer(); }
        else if (playerInSightRange && !playerInAttackRange) { ChasePlayer(); }
        else { Patrolling(); }
        
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        goblin = GetComponent<NavMeshAgent>();

        currentHealth = maxhealth;
        healthbar = GetComponentInChildren<Healthbar>();
        healthbar.UpdateHealthbar(currentHealth, maxhealth);
    }
    private void Patrolling()
    {
        if (!walkPointSet) { SearchWalkPoint(); }
        else
>>>>>>> 1f344ca0c61191bfcc704af4c56c49f79adde366
        {
            // Calculate the direction from the enemy to the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Move the enemy towards the player gradually
            Vector3 targetPosition = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            enemyRigidbody.MovePosition(targetPosition);

            // Rotate the enemy to face the player smoothly
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
<<<<<<< HEAD
=======

        Vector3 disctanceToWalkPoint = transform.position - walkPoint;

        //check if Walkpoint Reached
        if (disctanceToWalkPoint.magnitude < 1.0f)
        {
            walkPointSet = false;
        }

    }

    private void SearchWalkPoint()
    {   
        //calculate a random point in Range
        float randomZ = Random.Range(-walkpointRange, walkpointRange);
        float randomX = Random.Range(-walkpointRange, walkpointRange);

        //create Walkpoint
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //Check if Walkpoint is in Map
        if (Physics.Raycast(walkPoint,-transform.up,2f,whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        goblin.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Ensure Goblin doesnt move
        goblin.SetDestination(transform.position);

        transform.LookAt(player);
        if (!alreadyAttacked) {
            //Attack Code here





            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void TakeDamage(int Damage)
    {
        currentHealth -= Damage;
        healthbar.UpdateHealthbar(currentHealth, maxhealth);
        if (currentHealth< 0) { Invoke(nameof(GameObject), .5f); }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    //Visualize attack and Sight Range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, sightRange);
>>>>>>> 1f344ca0c61191bfcc704af4c56c49f79adde366
    }
}
