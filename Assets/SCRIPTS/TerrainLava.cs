using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLava : MonoBehaviour
{
    public int damage = 100;
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Trigger");
        if (collider.CompareTag("Player"))
        {
            Debug.Log(collider);
            collider.GetComponent<PlatformerCharaterController>().TakeDamage(damage);
        }
    }
}
