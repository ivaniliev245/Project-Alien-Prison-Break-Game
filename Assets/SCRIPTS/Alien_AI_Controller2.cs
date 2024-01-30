using UnityEngine;

public class Alien_AI_Controller2 : MonoBehaviour
{
    public Transform player;
    public LayerMask whatIsPlayer;

    public float attackRange;
    private bool playerInRange;

    public GameObject projectilePrefab;
    public Transform attackPoint;

    private Animator enemyAnimator;

    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if the player is in range
        playerInRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInRange)
        {
            // Face the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0; // Ensure no vertical movement
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Trigger the shooting animation
            if (enemyAnimator != null)
            {
                enemyAnimator.SetBool("Attack", true);
            }

            // Shoot
            if (!IsAnimationPlaying("Attack")) // Check if the Attack animation is not playing
            {
                Shoot();
            }
        }
        else
        {
            // Reset the Attack animation trigger
            if (enemyAnimator != null)
            {
                enemyAnimator.SetBool("Attack", false);
            }
        }
    }

void Shoot()
{
    // Instantiate the projectile
    GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, attackPoint.rotation);

    // Access the projectile's script (assuming it has one) to set the target position
    BulletShooting projectileScript = projectile.GetComponent<BulletShooting>();
    if (projectileScript != null)
    {
        // Set the target position (player's position)
        projectileScript.SetTargetPosition(player.position);
    }
}



    bool IsAnimationPlaying(string animationName)
    {
        // Check if the specified animation is playing
        if (enemyAnimator != null)
        {
            return enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        }

        return false;
    }
}
