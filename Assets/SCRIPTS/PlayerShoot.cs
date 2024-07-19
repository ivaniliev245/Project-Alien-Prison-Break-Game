using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Transform enemy; // Assign the enemy's transform in the Unity Editor
    public Transform shootPoint; // Assign the shoot point's transform in the Unity Editor
    public GameObject bulletPrefab; // Assign the bullet prefab in the Unity Editor
    public float bulletSpeed = 6f;
    public float currentwater = 1f;
    public float shootCooldown = 1f; // Adjustable delay between shots
    private bool canShoot = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && canShoot)
        {
            if (currentwater > 0)
            {
                StartCoroutine(Shoot());

                // Reduce current health in waterscript by 2
                waterscript script = GetComponent<waterscript>();
                if (script != null && script.currentHealth > 0)
                {
                    script.Updatewaterbar(script.currentHealth - 2f);
                }
            }
        }
        else if (currentwater <= 0)
        {
            // Do Nothing
        }
    }

    public void Updatewater(float currentwater)
    {
        // Debug.Log(currentwater);
        if (currentwater < 0)
        {
            this.currentwater = 0f;
        }
        else
        {
            this.currentwater = currentwater;
        }
    }

    IEnumerator Shoot()
    {
        // Set canShoot to false to start the cooldown
        canShoot = false;

        // Wait for a short moment (you can adjust this duration)
        yield return new WaitForSeconds(0.1f);

        // Instantiate bullet prefab from the shoot point to the enemy
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        bullet.transform.LookAt(enemy);

        // Add force or velocity to the bullet to make it move towards the enemy
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletSpeed, ForceMode.Impulse);

        // Start the cooldown timer
        yield return new WaitForSeconds(shootCooldown);

        // Allow shooting again after the cooldown
        canShoot = true;
    }
}
