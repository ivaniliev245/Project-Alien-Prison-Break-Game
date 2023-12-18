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

    private Animator enemyAnimator;
    
    public GameObject projectile;
    [SerializeField] private HealthbarEnemy healthbar;

    private bool isRunningBackward = false;
    private bool wasMovingBackward = false;

    public float backwardRunRange;
    public float rotationSpeed = 5f;

    public GameObject Bullet;
    public Transform attackPoint;

    void Update()
    {

        enemyAnimator = GetComponent<Animator>();


        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        bool isMovingBackward = IsMovingBackward();

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patrolling();
            SetAnimatorSpeed(0.5f); // Adjust speed for patrolling
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            SetAnimatorSpeed(1.0f); // Adjust speed for chasing
        }

        if (playerInSightRange && playerInAttackRange)
        {
            AttackPlayer();
            SetAnimatorSpeed(0.0f); // Adjust speed for attacking
        }

        if (playerInSightRange && Vector3.Distance(transform.position, player.position) < backwardRunRange)
        {
            RunBackward();
            SetAnimatorRunningBackward(true); // Trigger backward running animation
        }
        else
        {
            SetAnimatorRunningBackward(false);
            isRunningBackward = false;
        }

        if (isMovingBackward != wasMovingBackward)
        {
            SetAnimatorRunningBackward(isMovingBackward);
            wasMovingBackward = isMovingBackward;
        }
    }

    private void Awake()
    {
        backwardRunRange = 10.0f;
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        goblin = GetComponent<NavMeshAgent>();

        currentHealth = maxhealth;
        healthbar = GetComponentInChildren<HealthbarEnemy>();
        healthbar.UpdateHealthbar(currentHealth, maxhealth);
        enemyAnimator = GetComponent<Animator>();
    
    }
    private void Patrolling()
    {
        if (!walkPointSet) { SearchWalkPoint(); }
        if(walkPointSet)
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




    private void PerformAttack()
    {
        // Attack Code here
        // For example, instantiate a projectile or perform melee attack

        // Example with projectile:
        Rigidbody rb = Instantiate(projectile, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 32f, ForceMode.Impulse);

        // Damage code
        BulletShooting projectileScript = rb.GetComponent<BulletShooting>();

        // Ensure to set the Attack bool to false after the attack animation finishes
        if (enemyAnimator != null)
        {
            // Delay the resetting of the "Attack" parameter after a certain duration
            Invoke(nameof(ResetAttackAnimation), 1.0f); // Adjust the time according to your attack animation duration
        }
    }
    
    
    
    private void ResetAttackAnimation()
{
    // Reset the Attack parameter in the Animator after the attack animation finishes
    if (enemyAnimator != null)
    {
        enemyAnimator.SetBool("Attack", false);
    }
}


    private void AttackPlayer()
    {
        // Ensure Goblin doesn't move
        goblin.SetDestination(transform.position);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Ensure no vertical movement

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Smoothly rotate the upper body towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (!alreadyAttacked)
        {
            // Trigger Attack animation
            if (enemyAnimator != null)
            {
                enemyAnimator.SetBool("Attack", true); // Trigger the Attack animation
            }

            // Delay the attack
            Invoke(nameof(PerformAttack), 0.5f); // Adjust the delay time if needed


            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private bool IsMovingBackward()
    {
        if (goblin.velocity != Vector3.zero)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(goblin.velocity);
            return localVelocity.z < 0; // Check if the local z velocity is negative (moving backward)
        }

        return false;
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

    private void SetAnimatorSpeed(float speed)
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetFloat("speedEnemy", speed); // Set the speed parameter in Animator
        }
        else
        {
            Debug.LogError("Animator component not found!");
        }
    }

    private void RunBackward()
    {
        // Implement backward running logic here
        //
        //transform.Translate(-transform.forward * backwardRunSpeed * Time.deltaTime);
        isRunningBackward = true;
    }

    private void SetAnimatorRunningBackward(bool value)
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("runningBackward", value); // Set the "runningBackward" parameter
        }
        else
        {
            Debug.LogError("Animator component not found!");
        }
    }

}
