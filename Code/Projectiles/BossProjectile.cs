using System;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 5f; // Speed of the projectile
    private Vector2 direction; // Initial direction of the projectile
    private Rigidbody2D rb;
    private Animator animator; // Reference to animator for hit animation
    private bool hitAnimation = false; // Boolean to track hit state
    public int damage = 2; // Damage dealt by the projectile

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Ensure the projectile has an Animator component
        rb.velocity = direction * speed; // Set initial velocity for straight shot
    }

    // Set the initial direction from the boss
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage); 
                PlayHitAnimation();
            }
        }

        Destroy(gameObject); // Destroy the projectile on collision
    }

    private void PlayHitAnimation()
    {
        if (!hitAnimation)
        {
            hitAnimation = true; // Prevents triggering the animation multiple times
            animator.SetBool("Hit", true); // Assuming "Hit" is the name of the hit animation trigger

            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length); // Destroy after animation
        }
    }
}