using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;

    public Transform attackPoint; // Point where player deals damage
    public float attackRange;
    public float timeBetweenAttacks;
    private bool alreadyAttacked = false;

    public int attackDamage = 25;

    public LayerMask whatIsEnemy;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!alreadyAttacked)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKey(KeyCode.JoystickButton2))
            {
                //Debug.Log("pressed Attack");
                Attack();
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }
    
    private void Attack()
    {
        //play Attack Animation

        //detect Enemies
        Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackRange, whatIsEnemy);

        //damage Enemies
        foreach(Collider enemy in enemiesHit)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Alien_AI_Controller>().TakeDamage(attackDamage);
            }
            else if (enemy.CompareTag("piccolo"))
            {
                Debug.Log("hit");
                enemy.GetComponent<PiccoloHealthBar>().TakeDamage((float) attackDamage);
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }
}