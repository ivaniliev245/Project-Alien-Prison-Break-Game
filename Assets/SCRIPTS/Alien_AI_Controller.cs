using UnityEngine;
using UnityEngine.AI;

public class Alien_AI_Controller : MonoBehaviour
{
    public NavMeshAgent goblin;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    //patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkpointRange;

    //Attackspeed  
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    bool playerInSightRange, playerInAttackRange;

    //Health
    public float health;

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

    }
    private void Patrolling()
    {
        if (!walkPointSet) { SearchWalkPoint(); }
        else
        {
            goblin.SetDestination(walkPoint);
        }

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
        health -= Damage;
        if (health < 0) { Invoke(nameof(GameObject), .5f); }
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
    }
}
