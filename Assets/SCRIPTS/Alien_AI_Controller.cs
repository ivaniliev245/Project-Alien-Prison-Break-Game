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
    
    //for some reason seems to break the ai
    public bool SetPatrolling;  //Needs one of SetPatrollingX or -Y to be eneabled
    public bool SetPatrollingX; //ONLY WORKS WITH SET PATROLLING. if true enemy only patrolls on X axis, if false on x and z axis.
    public bool SetPatrollingZ; //ONLY WORKS WITH SET PATROLLING. if true enemy only patrolls on Z axis, if false on x and z axis.
    private Vector3 spawnpoint;
    private bool posWalkpoint;

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
            goblin.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        Debug.Log("Distance: " + distanceToWalkPoint.magnitude);

        //check if Walkpoint Reached
        if (distanceToWalkPoint.magnitude < 1.0f)
        {
            walkPointSet = false;
        }

    }
    private void SearchWalkPoint() 
    {
        //Checks if Patrolling is disabled -> enemy moves completly random from point to point over the map
        if (!SetPatrolling)
        {   
            //check if a walkpoint is already set
            if (!walkPointSet)
            {
                //calculate a random point in Range
                float randomZ = Random.Range(-walkpointRange, walkpointRange);
                float randomX = Random.Range(-walkpointRange, walkpointRange);

                //create Walkpoint
                walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

                //Check if Walkpoint is in Map
                if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
                {
                    walkPointSet = true;
                }
            }
        }
        //Check if Patrolling is set
        if (SetPatrolling)
        {
            if (!walkPointSet)
            {
                //check if Patrolling on x and z axis
                if (SetPatrollingX && SetPatrollingZ)
                {
                    //look if the last walkpoint was negativ, if not create positive walkpoint
                    if (!posWalkpoint)
                    {
                        //create Walkpoint
                        walkPoint = new Vector3(spawnpoint.x + walkpointRange, spawnpoint.y, spawnpoint.z + walkpointRange);
                        posWalkpoint = true;
                    }
                    //if true create a negative walkpoint from spawnpoint
                    if (posWalkpoint)
                    {
                        walkPoint = new Vector3(spawnpoint.x - walkpointRange, spawnpoint.y, spawnpoint.z - walkpointRange);
                        posWalkpoint = false;
                    }
                }
                //check if only Patrolling on X axis
                if (SetPatrollingX && !SetPatrollingZ)
                {
                    //look if the last walkpoint was negativ, if not create positive walkpoint
                    if (!posWalkpoint)
                    {
                        //create Walkpoint
                        walkPoint = new Vector3(spawnpoint.x + walkpointRange, spawnpoint.y, spawnpoint.z);
                        posWalkpoint = true;
                    }
                    //if true create a negative walkpoint from spawnpoint
                    if (posWalkpoint)
                    {
                        walkPoint = new Vector3(spawnpoint.x - walkpointRange, spawnpoint.y, spawnpoint.z);
                        posWalkpoint = false;
                    }
                }
                //check if only Patrolling on Z axis
                if (!SetPatrollingX && SetPatrollingZ)
                {
                    //look if the last walkpoint was negativ, if not create positive walkpoint
                    if (!posWalkpoint)
                    {
                        //create Walkpoint
                        walkPoint = new Vector3(spawnpoint.x, spawnpoint.y, spawnpoint.z + walkpointRange);
                        posWalkpoint = true;
                    }
                    //if true create a negative walkpoint from spawnpoint
                    if (posWalkpoint)
                    {
                        walkPoint = new Vector3(spawnpoint.x, spawnpoint.y, spawnpoint.z - walkpointRange);
                        posWalkpoint = false;
                    }
                }
            }
        }
        //Check if Walkpoint is in Map then set walkPointSet true
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }
    private void SearchWalkPointOld()
    {
        //look if ai has a set walkpoint
        if (!walkPointSet && !SetPatrolling)
        {
            //calculate a random point in Range
            float randomZ = Random.Range(-walkpointRange, walkpointRange);
            float randomX = Random.Range(-walkpointRange, walkpointRange);

            //create Walkpoint
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            //Check if Walkpoint is in Map
            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            {
                walkPointSet = true;
            }
            return;
        }

        //if walkpoint set is true
        //look if the last walkpoint was negativ
        if (!posWalkpoint)
        {
            //create Walkpoint
            walkPoint = new Vector3(spawnpoint.x + walkpointRange, spawnpoint.y, spawnpoint.z + walkpointRange);
            posWalkpoint = true;
        }
        else if (posWalkpoint)
        {
            walkPoint = new Vector3(spawnpoint.x - walkpointRange, spawnpoint.y, spawnpoint.z - walkpointRange);
            posWalkpoint = false;
        }

        //Check if Walkpoint is in Map
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
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
