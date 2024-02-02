using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPoolScript : MonoBehaviour
{
    public int damageAmount = 10;
    public float destroyDelay = 5.0f;

    void Update()
    {
        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter(Collider enemy)
    {
        if (enemy.CompareTag("Enemy"))
        {
            enemy.GetComponent<Alien_AI_Controller>().TakeDamage(damageAmount);
            Destroy(gameObject);
        }
        else if (enemy.CompareTag("piccolo"))
        {
            enemy.GetComponent<PiccoloHealthBar>().TakeDamage((float) damageAmount);
            Destroy(gameObject);
        }
    }
}