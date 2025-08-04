using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 1f;
    public GameObject player;
    private Rigidbody2D myRigidbody2D;
    private Animator _animator;

    private Vector2 startingPosition;
    private bool returningToStart = false;

    private float distance;
    private float minAttackDistance = 1f; // Minimum distance to stop moving toward the player
    public float detectionRange = 5f;
    public float catchDistance = 0.5f;

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
    public float attackCooldown = 3f;
    private bool canAttack = true;

    // Patrolling variables
    public Transform[] patrolPoints;
    private int currentPatrolPointIndex = 0;
    private bool waitingAtPoint = false;
    private float waitTime = 2f;
    private float waitTimer = 0f;

    // Sounds
    public GameObject TrappedSound;
    public GameObject DyingSound;

    // Item drop
    [SerializeField] private GameObject itemDropPrefab; // Add a reference to the item prefab
    [SerializeField] private Transform dropPoint; // Drop point for the item

    public static event Action OnBossDefeated;
    private bool canChasePlayer = true;

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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
        
        Gizmos.color = Color.green;
        // Draw all the patrol points
        foreach (Transform point in patrolPoints)
        {
            Gizmos.DrawWireSphere(point.position, 1f);
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return; // Skip update if the enemy is dead
        if (isTakingDamage) return;
        distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < detectionRange)
        {
            if (canChasePlayer)
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

        if (myRigidbody2D.velocity.x > 0 && !isFacingRight())
        {
            Flip();
        }
        else if (myRigidbody2D.velocity.x < 0 && isFacingRight())
        {
            Flip();
        }

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
        
        if (distance < catchDistance && canAttack)
        {
            _animator.SetBool("Attack", true);
            StartCoroutine(AttackPlayer());
        }

        returningToStart = false;
        waitingAtPoint = false;
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false; 
        
        // Stop the movement and play the attack animation
        
        StopMovement();
        
        _animator.SetBool("Attack", true);

        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        player.GetComponent<Player>().TakeDamage(damage);

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        _animator.SetBool("Attack", false);
    }
    

    public void TakeDamage(int damage)
    {
        if (isDead || isTakingDamage) return;

        Health -= damage;
        TrappedSound.SetActive(true);

        isTakingDamage = true;
        StopMovement();
        _animator.SetBool(_Damage, true);

        if (Health <= 0)
        {
            _animator.SetBool(_Died, true);
            StartCoroutine(HandleDeath());
        }
        else
        {
            StartCoroutine(ResetDamageAnimation());
        }
    }

    private void StopMovement()
    {
        myRigidbody2D.velocity = Vector2.zero;
    }

    private IEnumerator HandleDeath()
    {
        DyingSound.SetActive(true);
        isDead = true;
        GetComponent<Boss3OtherCode>().enabled = false;
        myRigidbody2D.velocity = Vector2.zero;
        OnBossDefeated?.Invoke();
        GameEventsManagerSO.instance.miscEvents.Boss3Defeated();

        // Call the event to trigger the boss death subscriptions (Dialogue, Quest, etc.)
        GameEventsManagerSO.instance.miscEvents.DialogueRequested(true, "Outro");
        
        // Wait for the dialogue to end
        bool dialogueEnded = false;

        GameEventsManagerSO.instance.miscEvents.OnDialogueEnded += OnDialogueEnded;
        yield return new WaitUntil(() => dialogueEnded);
        GameEventsManagerSO.instance.miscEvents.OnDialogueEnded -= OnDialogueEnded;
        
        int randomExp = Random.Range(1, 4);
        LevelingSystem.CurrentXp += randomExp;
        LevelingSystem.ExperienceController();
        
        DropItem(); // Call the method to drop the item after boss death
        gameObject.SetActive(false);
        
        yield break;

        void OnDialogueEnded()
        {
            dialogueEnded = true;
        }
    }

    private void DropItem()
    {
        if (itemDropPrefab != null && dropPoint != null)
        {
            Instantiate(itemDropPrefab, dropPoint.position, Quaternion.identity); // Instantiate the item at the drop point
        }
    }

    private IEnumerator ResetDamageAnimation()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _animator.SetBool(_Damage, false);
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
        if (patrolPoints.Length == 0) return;

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
    
    private bool circleColliderTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !circleColliderTriggered)
        {
            circleColliderTriggered = true;
            //StartCoroutine(HandleDialogue());
            
            // Stop the boss from chasing the player
            canChasePlayer = false;
            myRigidbody2D.velocity = Vector2.zero;
            
            // Call the dialogue event to trigger the boss dialogue
            StartCoroutine(HandleDialogue());
        }
    }
    
    private IEnumerator HandleDialogue()
    {
        // Change the rigidbody to static to prevent it from moving
        //StopCoroutine(nameof(AttackPlayer));

        // Call the event to trigger the boss dialogue
        GameEventsManagerSO.instance.miscEvents.DialogueRequested(true, "Intro");

        // Wait for the dialogue to end
        bool dialogueEnded = false;

        GameEventsManagerSO.instance.miscEvents.OnDialogueEnded += OnDialogueEnded;
        yield return new WaitUntil(() => dialogueEnded);
        GameEventsManagerSO.instance.miscEvents.OnDialogueEnded -= OnDialogueEnded;
        
        // Resume the boss movement
        canChasePlayer = true;
        
        // Enable the boss attack component
        GetComponent<Boss3OtherCode>().enabled = true;
        
        yield break;

        void OnDialogueEnded()
        {
            dialogueEnded = true;
        }
    }
}
