using UnityEngine;
using UnityEngine.AI;

public class Alien_AI_Controller : MonoBehaviour
{
    public NavMeshAgent goblin;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    //patroling
    private Vector3 walkPoint;
    bool walkPointSet;
    public float walkpointRange;
    
    private Vector3 spawnpoint;

    //Attackspeed  
    public float timeBetweenAttacks;
    bool alreadyAttacked;

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

    private float backwardRunRange;
    public float rotationSpeed = 5f;
    //public float walkSpeed = 0.5f;

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
        backwardRunRange = attackRange;
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        goblin = GetComponent<NavMeshAgent>();

        currentHealth = maxhealth;
        healthbar = GetComponentInChildren<HealthbarEnemy>();
        healthbar.UpdateHealthbar(currentHealth, maxhealth);
        enemyAnimator = GetComponent<Animator>();

        spawnpoint = transform.position;
    
    }
    private void Patrolling()
    {
        if (!walkPointSet) { SearchWalkPoint(); }
        if(walkPointSet)
        {
            //Debug.Log("Destination Set");
            goblin.SetDestination(walkPoint);
        }
        float distanceToWalkPoint = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(walkPoint.x, 0, walkPoint.z));

        //check if Walkpoint Reached
        if (distanceToWalkPoint < 1.0f)
        {
            walkPointSet = false;
        }

    }

    private bool IsInWalkRange(Vector3 position)
    {
        // Calculate the boundaries of the box
        float minX = spawnpoint.x - walkpointRange;
        float maxX = spawnpoint.x + walkpointRange;
        float minZ = spawnpoint.z - walkpointRange;
        float maxZ = spawnpoint.z + walkpointRange;

        // Check if the position is within the box
        return (position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ);
    }

    private void SearchWalkPoint()
    {
        //check if a walkpoint is already set
        if (!walkPointSet)
        {
            //calculate a random point in Range
            float randomX = Random.Range(-walkpointRange, walkpointRange);
            float randomZ = Random.Range(-walkpointRange, walkpointRange);

            //create Walkpoint
            walkPoint = new Vector3(spawnpoint.x + randomX, transform.position.y, spawnpoint.z + randomZ);

            //Check if Walkpoint is in Map
            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround) && IsInWalkRange(walkPoint))
            {
                walkPointSet = true;
            }
        }
    }
    private void ChasePlayer()
    {
        goblin.SetDestination(player.position);
    }
    private void PerformAttack()
    {
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
        //transform.Translate(-transform.forward * backwardRunSpeed * Time.deltaTime);
        isRunningBackward = true;

        Invoke(nameof(ResetRunningBackward), 1.0f);//Adjust Time to Animation Speed
    }

    private void ResetRunningBackward()
    {
        isRunningBackward = false;
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
