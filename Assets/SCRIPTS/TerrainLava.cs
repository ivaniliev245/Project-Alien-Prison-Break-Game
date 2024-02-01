using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLava : MonoBehaviour
{
    public int damage = 999;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.GetComponent<PlatformerCharaterController>().TakeDamage(damage);
        }
    }
}
