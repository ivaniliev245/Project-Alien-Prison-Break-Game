using UnityEngine;

public class GoodFellas : MonoBehaviour
{
    public Transform player; 
    public float followDistance = 5f; 
    public float stopDistanceFromPlayer = 1f; 
    public float enemyDetectionDistance = 5f; 
    public float maxChaseDistance = 10f; 
    public LayerMask enemyLayer; 
    public float movementSpeed = 4f; 
    public GameObject attackVFXPrefab; 
    public GameObject fireVFXPrefab; 
    public Transform dragonMouth; 
    public float vfxDuration = 2f; // Duration of VFX before it's destroyed

    private Transform currentTarget; 
    private Animator animator; 
    private bool isFollowingEnemy; 
    private bool isAttacking; 
    private GameObject attackVFXInstance; 

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        animator = GetComponentInChildren<Animator>();

        isFollowingEnemy = false;
        isAttacking = false;

        if (attackVFXPrefab != null && dragonMouth != null)
        {
            attackVFXInstance = Instantiate(attackVFXPrefab, dragonMouth.position, Quaternion.identity, dragonMouth);
            attackVFXInstance.SetActive(false);
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionDistance, enemyLayer);
        bool enemyDetected = colliders.Length > 0;
        Transform nearestEnemy = null;

        if (enemyDetected)
        {
            nearestEnemy = colliders[0].transform;
        }

        if (nearestEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, nearestEnemy.position);

            if (distanceToEnemy > maxChaseDistance)
            {
                currentTarget = player;
                isFollowingEnemy = false;
            }
            else
            {
                if (distanceToEnemy <= followDistance)
                {
                    currentTarget = nearestEnemy;
                    isFollowingEnemy = true;
                }
                else
                {
                    currentTarget = player;
                    isFollowingEnemy = false;
                }
            }
        }
        else
        {
            currentTarget = player;
            isFollowingEnemy = false;
        }

        if (currentTarget != null)
        {
            if (currentTarget == player && distanceToPlayer <= stopDistanceFromPlayer)
            {
                animator.SetFloat("Speed", 0.1f);
            }
            else
            {
                MoveTowardsTarget(currentTarget);
                animator.SetFloat("Speed", 1f);
            }
        }

        animator.SetBool("Attack", isAttacking);
    }

    void MoveTowardsTarget(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * movementSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            isAttacking = true;

            if (attackVFXInstance != null)
            {
                attackVFXInstance.SetActive(true);
            }

            if (fireVFXPrefab != null)
            {
                GameObject fireVFXInstance = Instantiate(fireVFXPrefab, other.transform.position, Quaternion.identity);
                fireVFXInstance.transform.parent = other.transform; // Make the VFX a child of the enemy
                Destroy(fireVFXInstance, vfxDuration); // Destroy the VFX after the specified duration
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            isAttacking = false;

            if (attackVFXInstance != null)
            {
                attackVFXInstance.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, followDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, stopDistanceFromPlayer);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyDetectionDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
        }
    }
}
