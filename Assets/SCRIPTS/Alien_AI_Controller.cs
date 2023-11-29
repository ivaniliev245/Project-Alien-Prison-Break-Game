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
    public int attackDamage;

    //States
    public float sightRange, attackRange;
    bool playerInSightRange, playerInAttackRange;

    //Health
    public float maxhealth;
    private float currentHealth;

    public GameObject projectile;
    [SerializeField] private Healthbar healthbar;


    void Update()
    {
        //Check if Player is in sight- and AttackRange
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) { Patrolling(); }
        if (playerInSightRange && !playerInAttackRange) { ChasePlayer(); }
        if (playerInSightRange && playerInAttackRange) { AttackPlayer(); }
        
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
        if(walkPointSet)
        {
            goblin.SetDestination(walkPoint);
            //Call Walking Animation 
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
            Rigidbody rb =  Instantiate(projectile,transform.position,Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);

            //Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackRange, whatIsPlayer);



            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage;
        healthbar.UpdateHealthbar(currentHealth, maxhealth);

        //play enemy damage animation
        if (currentHealth<= 0) { Invoke(nameof(DestroyEnemy), .5f); }
    }

    private void DestroyEnemy()
    {
        //play enemy death animation
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
