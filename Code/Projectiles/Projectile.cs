using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D projectileRB;
    public float speed;
    public float projectileLife;
    public float projectileCount;
    private Vector2 _direction;
    private Animator _animator;
    private bool HitAnimation = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        projectileCount = projectileLife;
        projectileRB.velocity = _direction * speed;
    }

    public void SetDirection(Vector2 direction)
    {
        _direction = direction.normalized;
    }
    // Update is called once per frame
    void Update()
    {
        projectileCount -= Time.deltaTime;
        if (projectileCount <= 0)
        {
            HitAnimation = true;
            Destroy(gameObject);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            AIchase enemyAI = other.gameObject.GetComponent<AIchase>();
            Boss boss3 = other.gameObject.GetComponent<Boss>();
            if (enemyAI != null)
            {
                // Deal damage to the enemy
                enemyAI.TakeDamage(3);  // Adjust the damage value as needed

            }

            if (boss3 != null)
            {
                boss3.TakeDamage(3);
            }
        }

        HitAnimation = true;
        Destroy(gameObject);
    }
}
