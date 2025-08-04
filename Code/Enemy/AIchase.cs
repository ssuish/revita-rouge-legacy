using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AIchase : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    public GameObject player;
    private Rigidbody2D myRigidbody2D;
    private Animator _animator;

    private Vector2 startingPosition;
    private bool returningToStart = false;

    private float distance;
    private float minAttackDistance = 1f; // Minimum distance to stop moving toward the player
    public float detectionRange = 4f;
    private float catchDistance = 1.5f;

    //GivePlayerEXp
    public Leveling LevelingSystem;

    // Health-related variables
    public int Health = 8;
    private bool isDead = false;
    private bool isTakingDamage = false;
    private const string _Damage = "EnemyDamage";
    private const string _Died = "EnemyDied";

    // Attack-related variables 
    public int damage = 1;
    public float attackCooldown = 3f; // Time in seconds between attacks
    private bool canAttack = true;

    // Patrolling variables
    public Transform[] patrolPoints;
    private int currentPatrolPointIndex = 0;
    private bool waitingAtPoint = false;
    private float waitTime = 2f;
    private float waitTimer = 0f;

    //sounds
    public GameObject TrappedSound;
    public GameObject DyingSound;

    private void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        startingPosition = transform.position;
        TrappedSound.SetActive(false);
        DyingSound.SetActive(false);
    }

    public void Awake()
    {
        // Automatically assign the Leveling component if not already assigned in the Inspector
        if (LevelingSystem == null)
        {
            LevelingSystem = GetComponent<Leveling>();
            if (LevelingSystem == null)
            {
                Debug.LogError("Leveling component not found on the Player object.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw the catch distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
        
        // Draw patrol points
        if (patrolPoints.Length > 0 || patrolPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform point in patrolPoints)
            { Gizmos.DrawWireSphere(point.position, .5f); // Draw a small sphere at each point
            }
        }
    }

    private void Update()
    {
        if (isDead) return; // Skip update if the enemy is dead
        if (isTakingDamage) return;
        distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < detectionRange)
        {
            ChasePlayer();
        }
        else if (returningToStart)
        {
            ReturnToStart();
        }
        else
        {
            Patrol();
        }

        // Adjust the facing direction and set animations
        if (myRigidbody2D.velocity.x > 0 && !isFacingRight())
        {
            Flip();
        }
        else if (myRigidbody2D.velocity.x < 0 && isFacingRight())
        {
            Flip();
        }

        // Set animator parameters
        _animator.SetFloat("Horizontal", myRigidbody2D.velocity.x);
        _animator.SetFloat("Vertical", myRigidbody2D.velocity.y);
    }

    private void ChasePlayer()
    {
        // Calculate the distance to the player
        float distance = Vector2.Distance(player.transform.position, transform.position);

        // Check if the enemy is within the minimum distance but still out of attack range
        if (distance > minAttackDistance)
        {
            // Move towards the player until the minAttackDistance is reached
            Vector2 direction = (player.transform.position - transform.position).normalized;
            myRigidbody2D.velocity = direction * moveSpeed;
        }
        else
        {
            // Stop moving once within the minimum attack distance
            myRigidbody2D.velocity = Vector2.zero;
        }

        // If within attack range, trigger the attack
        if (distance < catchDistance && canAttack)
        {
            //Debug.Log("Attacking player");
            _animator.SetBool("Attack", true);
            StartCoroutine(AttackPlayer());
        }

        returningToStart = false;
        waitingAtPoint = false; // Ensure not waiting at patrol points when chasing
    }


    private IEnumerator AttackPlayer()
    {
        canAttack = false; // Prevent further attacks until cooldown is over

        StopMovement();
        _animator.SetBool("Attack", true); // Trigger the attack animation

        // Wait for the attack animation to finish
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        // Recalculate the distance to the player
        var distanceRecalculate = Vector2.Distance(player.transform.position, transform.position);

        // Apply damage only if the player is still within the attack range
        if (distanceRecalculate < catchDistance)
        {
            player.GetComponent<Player>().TakeDamage(damage); // Apply damage after animation
        }

        // Wait for the cooldown duration
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true; // Allow attacks again after cooldown
        _animator.SetBool("Attack", false);
    }


    public void TakeDamage(int damage)
    {
        if (isDead || isTakingDamage) return; // Prevent taking damage if already dead or already taking damage

        Health -= damage;
        TrappedSound.SetActive(true);

        // Set the flag to true to stop movement during damage animation
        isTakingDamage = true;

        // Stop the enemy's movement
        StopMovement();

        // Set the damage animation to true
        _animator.SetBool(_Damage, true);

        // If health is 0 or less, start the death sequence
        if (Health <= 0)
        {
            _animator.SetBool(_Died, true);
            StartCoroutine(HandleDeath());
        }
        else
        {
            // Start a coroutine to reset the damage animation and movement after it plays
            StartCoroutine(ResetDamageAnimation());
        }
    }


    private void StopMovement()
    {
        // Stop movement by setting velocity to zero
        myRigidbody2D.velocity = Vector2.zero;
    }
    

    private IEnumerator HandleDeath()
    {
        //Debug.Log("Death coroutine started");
        DyingSound.SetActive(true);
        isDead = true;
        myRigidbody2D.velocity = Vector2.zero; // Stop movement
        yield return new WaitForSeconds(1f); // Wait for the death animation to play

        // Get player's current XP level and area
        int playerLevel = LevelingSystem.Level;
        //Debug.Log("Player Level: " + playerLevel);

        Player playerScript = FindObjectOfType<Player>(); // Assuming there's only one player in the scene
        if (playerScript == null)
        {
            //Debug.LogError("Player script not found in the scene.");
            yield break;
        }

        int area = GetPlayerArea(); // Access the area from the player script
        //Debug.Log("Player Area: " + area);

        int minExp = 0;
        int maxExp = 0;
        
        
        // Assign the min and max experience based on the player's level and the area
        if (playerLevel <= 5)
        {
            switch (area)
            {
                case 1:
                    minExp = 1;
                    maxExp = 3;
                    break;
                case 2:
                    minExp = 5;
                    maxExp = 6;
                    break;
                case 3:
                    minExp = 8;
                    maxExp = 8;
                    break;
                case 4:
                    minExp = 8;
                    maxExp = 8;
                    break;
            }
        }
        else if (playerLevel <= 10)
        {
            switch (area)
            {
                case 1:
                    minExp = 1;
                    maxExp = 2;
                    break;
                case 2:
                    minExp = 3;
                    maxExp = 5;
                    break;
                case 3:
                    minExp = 6;
                    maxExp = 7;
                    break;
                case 4:
                    minExp = 7;
                    maxExp = 7;
                    break;
            }
        }
        else if (playerLevel <= 15)
        {
            switch (area)
            {
                case 1:
                    minExp = 1;
                    maxExp = 1;
                    break;
                case 2:
                    minExp = 2;
                    maxExp = 4;
                    break;
                case 3:
                    minExp = 3;
                    maxExp = 5;
                    break;
                case 4:
                    minExp = 6;
                    maxExp = 6;
                    break;
            }
        }
        else if (playerLevel <= 20)
        {
            switch (area)
            {
                case 1:
                    minExp = 1;
                    maxExp = 1;
                    break;
                case 2:
                    minExp = 2;
                    maxExp = 3;
                    break;
                case 3:
                    minExp = 2;
                    maxExp = 4;
                    break;
                case 4:
                    minExp = 5;
                    maxExp = 5;
                    break;
            }
        }

        if (LevelingSystem.Level < 20)
        {
            // Randomly assign experience points within the range
            int randomExp = Random.Range(minExp, maxExp);
            LevelingSystem.CurrentXp += randomExp;
        }

        Player player = FindObjectOfType<Player>();
        //player.IncrementEnemiesKilled();

        // Subscribe to on enemy death event
        GameEventsManagerSO.instance.enemyEvents.EnemyDefeated();

        LevelingSystem.ExperienceController();
        gameObject.SetActive(false); // Destroy the enemy object
        yield return new WaitForSeconds(1f);
        RespawnEnemy();
    }

    private void RespawnEnemy()
    {
        // Reset health and any other status-related variables
        Health = 8; // Reset health to its initial value
        isDead = false;
        isTakingDamage = false;

        // Reset animator parameters
        _animator.SetBool(_Died, false);
        _animator.SetBool(_Damage, false);
        _animator.SetBool("Attack", false);

        // Reset position (you can respawn at a different location if needed)
        transform.position = startingPosition;

        // Reset patrol points, attack cooldown, etc. if needed

        // Reactivate the enemy
        gameObject.SetActive(true);
    }


    // Implement this function to return the correct area based on the player's position or other criteria
    private int GetPlayerArea()
    {
        // Get the current active scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Check the scene name or build index and return the corresponding area number
        switch (currentScene.name) // or use currentScene.buildIndex
        {
            case "PlayArea1": //scene name = Area 1
                return 1;
            case "Area2SceneName": //scene name = Area 2
                return 2;
            case "Area3SceneName": //scene name = Area 3
                return 3;
            case "BossAreaSceneName": //scene name = Area 4
                return 4;
            default:
                return 1; // Default to area 1 if no match found
        }
    }


    private IEnumerator ResetDamageAnimation()
    {
        // Wait for the length of the damage animation
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        // Reset the damage animation flag
        _animator.SetBool(_Damage, false);

        // Allow the enemy to move again
        isTakingDamage = false;
    }

    private void ReturnToStart()
    {
        if ((Vector2)transform.position != startingPosition)
        {
            Vector2 direction = (startingPosition - (Vector2)transform.position).normalized;
            myRigidbody2D.velocity = direction * moveSpeed;
        }
        else
        {
            myRigidbody2D.velocity = Vector2.zero;
            returningToStart = false;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        if (!waitingAtPoint)
        {
            Transform targetPoint = patrolPoints[currentPatrolPointIndex];
            Vector2 direction = (targetPoint.position - transform.position).normalized;
            myRigidbody2D.velocity = direction * moveSpeed;

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                myRigidbody2D.velocity = Vector2.zero;
                waitingAtPoint = true;
                waitTimer = waitTime;
            }
        }
        else
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waitingAtPoint = false;
                currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            }
        }

        returningToStart = false;
    }

    private bool isFacingRight()
    {
        return transform.localScale.x > 0;
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}