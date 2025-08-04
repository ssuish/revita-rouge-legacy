using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3OtherCode : MonoBehaviour
{
    public GameObject bullet;
    public Transform[] bulletPos; // Array of bullet spawn positions
    public float detectionRange = 10f; // Range within which the boss starts shooting

    private float timer;
    private Transform playerTransform;

    private void Start()
    {
        // Find the player by tag
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Only shoot if the player is within the detection range
        if (Vector2.Distance(transform.position, playerTransform.position) < detectionRange)
        {
            if (timer > 5f) // Boss shoots every 4 seconds
            {
                timer = 0;
                ShootProjectiles();
            }
        }
    }

    void ShootProjectiles()
    {
        foreach (Transform pos in bulletPos)
        {
            // Instantiate bullet at each position in the array
            GameObject spawnedBullet = Instantiate(bullet, pos.position, Quaternion.identity);

            // Set the direction for each bullet towards the player's current position
            BossProjectile projectile = spawnedBullet.GetComponent<BossProjectile>();
            if (projectile != null)
            {
                // Calculate direction from the bullet position to the player's position
                Vector2 directionToPlayer = (playerTransform.position - pos.position).normalized;
                projectile.SetDirection(directionToPlayer); // Set the projectile direction towards the player
            }
        }
    }
}