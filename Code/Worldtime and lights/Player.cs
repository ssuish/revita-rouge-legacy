using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    /*public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private Vector2 _movement;

    private void Update()
    {
      ProcessInputs();
    }

    private void FixedUpdate()
    {
       OnMove();
    }

    void ProcessInputs()
    {
       float moveX = Input.GetAxisRaw("Horizontal");
       float moveY = Input.GetAxisRaw("Vertical");


       _movement = new Vector2(moveX, moveY).normalized;
    }

    void OnMove()
    {
       rb.velocity = new(_movement.x * moveSpeed, _movement.y * moveSpeed);
    }
 */

    [SerializeField] private float _moveSpeed = 5f;
    public float Timer = 3;
    public List<GameObject> PickupItems = new List<GameObject>();

    private Vector2 _movement;
    private Rigidbody2D _rb;
    private Animator _animator;


    private InputManager _inputManager;
    //public ItemSpawner spawner;

    //animations
    private const string _horizontal = "Horizontal";
    private const string _vertical = "Vertical";
    private const string _lasthorizontal = "LastHorizontal";
    private const string _lastVertical = "LastVertical";
    private const string _isGathering = "IsGathering";
    private const string _Attacking = "Attacking";
    private const string _Died = "PlayerDie";

    //attacking
    private bool isAttacking = false;

    //projectile for attacking
    public GameObject projectilePrefab;
    public Transform launchpoint;
    public float shootTime;
    public float shootCounter;

    [SerializeField] private GameObject GatherBut;
    [SerializeField] private GameObject SprintBut;


    //health
    public int maxHealth = 8;
    public int Health;
    public int numOfHearts;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    //Exp
    public Leveling LevelingSystem;

    //crafting

    //sprinting
    public int maxHunger = 8; // Maximum sprint time (represented as "hunger" points)
    private int currentHunger;
    public int numOfHunger;
    public Image[] hungerBars; // Array of UI elements to represent hunger
    public Sprite fullHunger;
    public Sprite emptyHunger;
    public GameObject SprintingAudio;
    public GameObject walkingAudio;

    public float sprintDuration = 5f; // Total time you can sprint
    private float sprintTimer;
    private bool isSprinting = false;

    //flashlight
    public GameObject flashlight;
    private bool isFlashlightOn = false;

    //moving
    private Vector2 moveDir;

    //gathering
    public GameObject gatheringAudio;
    public bool isGathering;

    private PlayerActionMap _inputActions;

    //private ChangeSpriteOnCollision changeSpriteOnCollision;
    private DumpsInteraction dumpInteraction;
    private DatesInteraction datesInteraction;
    private LegumesInteractions legumesInteraction;

    private InventoryController _inventoryController;

    //changing of auto mata
    public AutomataChangingSprite automataChangingSprite;
    public GameObject upgradeBut;

    //Tracking of Player Death, Enemies killed, bosskilled
    public int deathCount = 0;
    public bool isDead = false;
    public int enemiesKilled = 0;
    public string boss2Name = "Seitune";
    public string boss3Name = "Hara";
    public bool isBoss2Killed = false;
    public bool isBoss3Killed = false;
    public int HasUsedHealthItems = 0;


    private void Start()
    {
        upgradeBut.SetActive(false);
        currentHunger = maxHunger;
        UpdateHungerDisplay();
        SprintingAudio.SetActive(false);
        gatheringAudio.SetActive(false);
        GatherBut.SetActive(false);
        SprintBut.SetActive(true);
        _inventoryController = InventoryController.Instance;
        // Initialize UI to full health and hunger at the start
        Health = numOfHearts;
        currentHunger = numOfHunger;

        UpdateHeartDisplay();
        UpdateHungerDisplay();
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _inputManager = GetComponent<InputManager>();
        _animator = GetComponent<Animator>();
        _inputActions = new PlayerActionMap();
        _inputActions.Player.Enable();
        Health = maxHealth;
        shootCounter = shootTime;
        
        if (mainMenuHolder == null)
        {
            mainMenuHolder = FindObjectOfType<MainMenuHolder>();
        }
        
        // Ensure the flashlight is off initially
        if (flashlight != null)
        {
            flashlight.SetActive(false);
        }

        _inventoryController = GetComponent<InventoryController>();
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

    private void Update()
    {
        // Update movement input
        if (isFlashlightOn)
        {
            UpdateFlashlightDirection();
        }
        
        // Flashlight toggle logic
        //if (Input.GetKeyDown(KeyCode.F))
        if (_inputActions.Player.Flashlight.triggered)
        {
            ToggleFlashlight();
        }

        //Pickup
        // Gathering with space bar
        //if (Input.GetKey(KeyCode.Space))
        /*if (_inputActions.Player.Gathering.triggered)
        {
           Gathering();
           //changeSpriteOnCollision.StartGathering();
           //dumpInteraction.DepletedSprite();
        }*/

        // Call Gathering logic if the button is held
        if (isGathering) // This will be triggered by the UI button
        {
            Gathering();
        }

        //walking
        Movement();
        
        //attacks
        //if (Input.GetKeyDown(KeyCode.E) && !isAttacking)
        if (_inputActions.Player.Attack.triggered && !isAttacking)
        {
            if (HasSnareInInventory())
            {
               StartCoroutine(Attack());
            }
        }
        
        if (_inputActions.Player.UseHealthItem.triggered)
        {
            UseHealthItem();
        }

        if (_inputActions.Player.UseStaminaItem.triggered)
        {
            UseHungerItem();
        }

        if (_inputActions.Player.UpgradeAutomata.triggered)
        {
            UpgradeSprite();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        UpdateLaunchPoint();
    }

    //gathering

    public void Gathering()
    {
        _animator.SetBool(_isGathering, true);
        Timer -= Time.deltaTime;
        gatheringAudio.SetActive(true);

        if (Timer <= 0)
        {
            // Iterate through the PickupItems and handle interactions based on the item type
            for (int i = PickupItems.Count - 1; i >= 0; i--)
            {
                if (PickupItems[i] != null)
                {
                    // Check for legumes interaction
                    if (PickupItems[i].CompareTag("Legume"))
                    {
                        legumesInteraction.HandleInteractionLegumes();
                    }
                    // Check for dates interaction
                    else if (PickupItems[i].CompareTag("Date"))
                    {
                        datesInteraction.HandleInteractionDates();
                    }
                    // Check for dump interaction (for items found in the dump)
                    else if (PickupItems[i].CompareTag("Dump"))
                    {
                        dumpInteraction.HandleInteraction();
                    }

                    // After interaction, proceed with picking up the item
                    PickupItems[i].GetComponent<Pickup>().PickupItem();

                    // Reward experience for the item gathered
                    if (LevelingSystem.Level < 20)
                    {
                        int randomExp = Random.Range(5, 10);
                        LevelingSystem.CurrentXp += randomExp;
                        LevelingSystem.ExperienceController();
                    }
                }
                else // Remove null items from the list
                {
                    PickupItems.RemoveAt(i);
                }
            }

            // Clear the PickupItems list after processing
            PickupItems.Clear();
            GatherBut.SetActive(false);

            // Reset the timer after gathering is complete
            Timer = 3;
        }
    }

    public void OnSubmitQuestPressed()
    {
        GameEventsManagerSO.instance.inputEvents.SubmitPressed();
    }

    public void OnQuestTogglePressed()
    {
        // Toggle the quest log UI
        GameEventsManagerSO.instance.inputEvents.QuestLogTogglePressed();
    }

    public void OnQuestProgressionTogglePressed()
    {
        GameEventsManagerSO.instance.inputEvents.QuestProgressionTogglePressed();
    }
    
    
    public void OnEndingTogglePressed()
    {
        GameEventsManagerSO.instance.inputEvents.EndingTogglePressed();
    }

    public void OnGatherButtonDown()
    {
        GatherBut.SetActive(true);
        isGathering = true; // Set this to true when button is held down
    }

    public void OnGatherButtonUp()
    {
        isGathering = false; // Set this to false when button is released
        gatheringAudio.SetActive(false); // Stop audio on release
        Timer = 3; // Reset the timer
        SprintBut.SetActive(true);
        //changeSpriteOnCollision.StopGathering();
    }

    private void LateUpdate()
    {
        //if (!isGathering && !Input.GetKey(KeyCode.Space))
        if (!isGathering && !_inputActions.Player.Gathering.triggered)
        {
            _animator.SetBool(_isGathering, false);
        }
    }

    private void HandleMovement()
    {
        // Check if sprinting
        float currentSpeed = isSprinting ? _moveSpeed * 1.5f : _moveSpeed;
        // Apply movement with calculated speed
        _rb.MovePosition(_rb.position + moveDir * (currentSpeed * Time.fixedDeltaTime));

        Sprinting();
    }

    //Movement
    public void Movement()
    {
        //_movement.Set(InputManager.Movement.x, InputManager.Movement.y);
        //moveDir = _inputManager.MoveInput.normalized;
        // Read joystick movement
        Vector2 joystickInput = _inputManager.MoveInput.normalized;

        // Set the movement direction based on joystick input
        moveDir = joystickInput;
        if (moveDir != Vector2.zero)
        {
            _animator.SetFloat(_horizontal, moveDir.x);
            _animator.SetFloat(_vertical, moveDir.y);
            _animator.SetFloat(_lasthorizontal, moveDir.x);
            _animator.SetFloat(_lastVertical, moveDir.y);
            walkingAudio.SetActive(true);
        }
        else
        {
            // No movement, reset animation parameters to zero
            _animator.SetFloat(_horizontal, 0);
            _animator.SetFloat(_vertical, 0);
            walkingAudio.SetActive(false);
        }
    }

    private string DetermineDirection(Vector2 movement)
    {
        if (moveDir == Vector2.zero) return "NO_MOVEMENT";
        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
        {
            return moveDir.x > 0 ? "RIGHT" : "LEFT";
        }

        return moveDir.y > 0 ? "UP" : "DOWN";
    }

    //sprinting
    public void Sprinting()
    {
        // Check if player is sprinting (holding shift or sprint button) and has enough hunger
        if (isSprinting)
        {
            if (currentHunger > 0)
            {
                // Increase speed and handle sprinting audio
                sprintTimer -= Time.deltaTime; // Decrease sprint timer

                SprintingAudio.SetActive(true);
                walkingAudio.SetActive(false);

                if (sprintTimer <= 0f) // If the sprint timer runs out
                {
                    DecreaseHunger(); // Decrease hunger points
                    sprintTimer = sprintDuration; // Reset sprint timer
                }
            }
            else
            {
                // Stop sprinting if hunger is depleted
                StopSprinting();
            }
        }
        else
        {
            // Not sprinting or hunger is zero
            SprintingAudio.SetActive(false);
            walkingAudio.SetActive(true);
        }

        // Update the hunger display
        UpdateHungerDisplay();
    }
    private void StopSprinting()
    {
        // Stop sprinting and reset necessary audio and flags
        isSprinting = false;
        SprintingAudio.SetActive(false);
        walkingAudio.SetActive(true);
    }

    public void OnSprintButtonDown()
    {
        // Called when the sprint button is pressed down
        isSprinting = true;
    }

    public void OnSprintButtonUp()
    {
        // Called when the sprint button is released
        isSprinting = false;
    }


    //health related
    private void UpdateHeartDisplay()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < Health)
            {
                hearts[i].sprite = fullHeart; // Set heart to full
            }
            else
            {
                hearts[i].sprite = emptyHeart; // Set heart to empty
            }

            // Enable only the hearts within max health
            hearts[i].enabled = i < numOfHearts;
        }
    }

    public void TakeDamage(int damage)
    {
        // Reduce the player's health by the damage taken
        Health -= damage;

        // Ensure health does not go below 0
        if (Health < 0)
        {
            Health = 0;
        }

        // Update the hearts display
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < Health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }

        // Handle death if health drops to or below 0
        if (Health <= 0)
        {
            _animator.SetBool(_Died, true);
            // Turn off the rigidbody to prevent movement
            _rb.bodyType = RigidbodyType2D.Static;
            StartCoroutine(HandleDeath());
        }
    }

    private void DecreaseHunger()
    {
        // Decrease hunger only if there's any left
        if (currentHunger > 0)
        {
            currentHunger--;
        }

        // Stop sprinting if hunger is depleted
        if (currentHunger <= 0)
        {
            currentHunger = 0;
            isSprinting = false; // Stop sprinting if out of hunger
        }

        // Ensure hunger bars are updated when hunger changes
        UpdateHungerDisplay();
    }

    private void UpdateHungerDisplay()
    {
        // Loop through hunger bars and update the sprite
        for (int i = 0; i < hungerBars.Length; i++)
        {
            if (i < currentHunger)
            {
                hungerBars[i].sprite = fullHunger; // Full bar if within current hunger
            }
            else
            {
                hungerBars[i].sprite = emptyHunger; // Empty bar if outside current hunger
            }

            // Enable only as many hunger bars as max hunger allows
            hungerBars[i].enabled = i < numOfHunger;
        }
    }


    public void UseHealthItem()
    {
        // Check if the player has a health item in their inventory
        if (_inventoryController.HasHealthItem())
        {
            // Check all inventory slots for health items and recover the correct amount of health
            Dictionary<string, int> inventoryItems = _inventoryController.CheckInventoryItems();

            // Loop through the inventory and recover health from the first health item found
            foreach (var item in inventoryItems)
            {
                // Check if the item is a health item
                if (_inventoryController.healthItems.ContainsKey(item.Key))
                {
                    int healthValue = _inventoryController.healthItems[item.Key]; // Get the health value
                    int itemCount = item.Value; // Number of items available

                    // Check if health is already full
                    if (IsHealthFull())
                    {
                        Debug.Log("Health is already full, cannot use health item.");
                        return; // Exit without using the item
                    }

                    // Add event listener to update Daily Quests. Get the item name.
                    GameEventsManagerSO.instance.playerEvents.PlayerEatenFood(item.Key);

                    // Recover health based on the health value of the item
                    RecoverHealth(healthValue);

                    

                    // Remove one instance of the health item from the inventory
                    _inventoryController.RemoveHealthItem(healthValue); // Pass the health value to remove
                    HasUsedHealthItems++;

                    Debug.Log($"Used health item: {item.Key}, Recovered: {healthValue} health");
                    return; // Exit after using the first health item
                }
            }
        }
        else
        {
            Debug.Log("No health items available in inventory.");
        }
    }
    // Add this helper function to check if health is full
    private bool IsHealthFull()
    {
        return Health >= maxHealth; // Replace with actual variables used for player health
    }

    
    // TODO - Add event listener to update Daily Quests.
    public void RecoverHealth(int healthAmount)
    {
        if (Health < maxHealth)
        {
            Health += healthAmount;

            // Ensure health doesn't exceed the maximum
            if (Health > maxHealth)
            {
                Health = maxHealth;
            }

            // Update the health display
            UpdateHeartDisplay();
        }
    }

    public void UseHungerItem()
    {
        // Check if the player has a hunger item in their inventory
        if (_inventoryController.HasHungerItem())
        {
            // Check all inventory slots for hunger items and recover the correct amount of hunger
            Dictionary<string, int> inventoryItems = _inventoryController.CheckInventoryItems();

            // Loop through the inventory and recover hunger from the first hunger item found
            foreach (var item in inventoryItems)
            {
                // Check if the item is a hunger item
                if (_inventoryController.StaminaItems.ContainsKey(item.Key))
                {
                    int hungerValue = _inventoryController.StaminaItems[item.Key]; // Get the hunger value
                    int itemCount = item.Value; // Number of items available

                    // Check if hunger is already full
                    if (IsHungerFull())
                    {
                        Debug.Log("Hunger is already full, cannot use hunger item.");
                        return; // Exit without using the item
                    }

                    // Add event listener to update Daily Quests. Get the item name.
                    GameEventsManagerSO.instance.playerEvents.PlayerEatenFood(item.Key);

                    // Recover hunger based on the hunger value of the item
                    RecoverHunger(hungerValue);

                    

                    // Remove one instance of the hunger item from the inventory
                    _inventoryController.RemoveHungerItem(hungerValue); // Pass the hunger value to remove
                    //currentHunger++; // Track the number of used hunger items

                    Debug.Log($"Used hunger item: {item.Key}, Recovered: {hungerValue} hunger");
                    return; // Exit after using the first hunger item
                }
            }
        }
        else
        {
            Debug.Log("No hunger items available in inventory.");
        }
    }

    // Add this helper function to check if hunger is full
    private bool IsHungerFull()
    {
        return currentHunger >= maxHunger; // Replace with actual variables used for player hunger
    }
    
    // TODO - Add event listener to update Daily Quests.
    public void RecoverHunger(int hungerAmount)
    {
        if (currentHunger < maxHunger) // Assuming you have a maxHunger variable
        {
            currentHunger += hungerAmount;

            // Ensure hunger doesn't exceed the maximum
            if (currentHunger > maxHunger)
            {
                currentHunger = maxHunger;
            }
            
            UpdateHungerDisplay(); // Assuming you have a method to update the hunger display
        }
    }


    //projectiles
    private void UpdateLaunchPoint()
    {
        if (_movement.x > 0) // Moving right
        {
            launchpoint.localPosition =
                new Vector3(0.5f, 0.5f, 0f); // Adjust these values to fit your player's sprite size
        }
        else if (_movement.x < 0) // Moving left
        {
            launchpoint.localPosition = new Vector3(-0.5f, 0f, 0f);
        }
        else if (_movement.y > 0) // Moving up
        {
            launchpoint.localPosition = new Vector3(0f, 0.5f, 0f);
        }
        else if (_movement.y < 0) // Moving down
        {
            launchpoint.localPosition = new Vector3(0f, -5f, 0f);
        }
    }

    //flashlight
    public void ToggleFlashlight()
    {
        if (flashlight != null)
        {
            isFlashlightOn = !isFlashlightOn;
            flashlight.SetActive(isFlashlightOn);
        }
    }

    private void UpdateFlashlightDirection()
    {
        // Determine the direction the player is facing
        Vector2 flashlightDirection;

        if (_movement != Vector2.zero)
        {
            // Player is moving, use the current movement direction
            flashlightDirection = _movement.normalized;
        }
        else
        {
            // Player is stationary, use the last recorded movement direction
            flashlightDirection = new Vector2(_animator.GetFloat(_lasthorizontal), _animator.GetFloat(_lastVertical));
        }

        // Lock the flashlight's Y position to a fixed offset (e.g., relative to the player's hand)
        Vector3 lockedYPosition = flashlight.transform.localPosition;
        lockedYPosition.y = -0.2f; // Set this value to the desired fixed Y offset for the flashlight

        // Adjust the X position based on the player's direction
        lockedYPosition.x = flashlightDirection.x * 0.2f; // Adjust the offset as needed

        // Update the flashlight position with the locked Y value
        flashlight.transform.localPosition = lockedYPosition;

        // For 2D rotation, rotate the flashlight to face the movement direction
        float angle = Mathf.Atan2(flashlightDirection.y, flashlightDirection.x) * Mathf.Rad2Deg;

        // Adjust by 90 degrees to correct the orientation
        angle -= 90f;

        flashlight.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    //Player Dies
    private IEnumerator HandleDeath()
    {
        

        _inventoryController.ClearInventory();
        _inventoryController.SaveInventory();
        // Increment the death count
        deathCount++;

        // Subscribe to the player death event
        GameEventsManagerSO.instance.playerEvents.PlayerDeath(deathCount);

        DisablePlayerControls();
        
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _inventoryController.SaveInventory();
        // Reset health and hearts before scene reload
        Health = numOfHearts; // Reset health to full
        UpdateHeartDisplay(); // Call a method to update the heart UI
        // Reset hunger to full
        currentHunger = numOfHunger;
        UpdateHungerDisplay(); // Call a method to update the hunger UI
        
        // Reset player rigidbody to allow movement
        _rb.bodyType = RigidbodyType2D.Dynamic;

        // Go to the automata base
        if (SceneManager.GetActiveScene().name == "Tutorial")
        { 
            GameEventsManagerSO.instance.miscEvents.SaveQuest();
            SceneManager.LoadScene("Tutorial");
        }
        else
        {
            GameEventsManagerSO.instance.miscEvents.SaveQuest();
            SceneManager.LoadScene("AutomataBase");
        }
    }

    // Method to check if the player has the Snare item
    private bool HasSnareInInventory()
    {
        // Assuming your inventory system has a method that checks for items
        return _inventoryController.HasItem("Snare");
    }

    public IEnumerator Attack()
    {
        isAttacking = true;
        _animator.SetBool(_Attacking, true);

        // Delay before firing the projectile (adjust this to match your animation)
        yield return new WaitForSeconds(0.5f); // Example delay, adjust based on your animation timing

        // Fire the projectile after the delay
        Vector2 direction = _movement != Vector2.zero
            ? _movement
            : new Vector2(_animator.GetFloat(_lasthorizontal), _animator.GetFloat(_lastVertical));
        FireProjectile(direction);


        // Remove 1 Snare from the inventory after firing
        InventoryController.Instance.RemoveItem("Snare", 1);

        // Wait for the remainder of the attack animation
        yield return new WaitForSeconds(0.5f); // Adjust as needed to match the rest of the animation

        _animator.SetBool(_Attacking, false);
        isAttacking = false;
    }

    private void FireProjectile(Vector2 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, launchpoint.position, Quaternion.identity);

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetDirection(direction);
    }

    public MainMenuHolder mainMenuHolder;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            PickupItems.Add(other.gameObject);

            if (PickupItems.Count > 0)
            {
                SprintBut.SetActive(false);
                GatherBut.SetActive(true);
            }
        }


        // If the player interacts with the dump
        if (other.CompareTag("Dump")) // Use appropriate tag or method to identify the dump interaction object
        {
            dumpInteraction = other.GetComponent<DumpsInteraction>();
            if (dumpInteraction != null)
            {
                //isGathering = true; // Now you can start gathering
            }
        }

        if (other.CompareTag("Automata")) // Ensure the object has the tag "Upgradable"
        {
            AutomataChangingSprite spriteSwitcher = other.GetComponent<AutomataChangingSprite>();
            if (spriteSwitcher != null)
            {
                upgradeBut.SetActive(true); // Show the upgrade button
                submitbutton.SetActive(false);
            }
        }

        if (other.CompareTag("IntroductionDone"))
        {
            mainMenuHolder.MarkIntroductionComplete();
            //Debug.Log("DoneIntro");
        }
    }

    public GameObject submitbutton;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Pickup"))
        {
            PickupItems.Remove(other.gameObject);

            if (PickupItems.Count == 0)
            {
                isGathering = false;
                OnGatherButtonUp();
                GatherBut.SetActive(false);
                SprintBut.SetActive(true);
            }
        }

        // Reset dumpInteraction when leaving the dump area
        /*if (other.CompareTag("Dump"))
        {
           if (other.GetComponent<DumpsInteraction>() == dumpInteraction)
           {
              dumpInteraction = null; // Remove reference to the dump interaction object
              isGathering = false;
           }
        }*/

        
        if (other.CompareTag("Automata")) // Ensure the object has the tag "Upgradable"
        {
            AutomataChangingSprite spriteSwitcher = other.GetComponent<AutomataChangingSprite>();
            if (spriteSwitcher != null)
            {
                upgradeBut.SetActive(false); // Show the upgrade button
                submitbutton.SetActive(true);
            }
        }
    }

    //conditions
    public void CheckGameConditions()
    {
        // Check if the player is dead
        if (isDead)
        {
            deathCount++;
            Debug.Log("Player has died " + deathCount + " times.");
        }

        // Check how many enemies have been killed
        Debug.Log("Enemies killed: " + enemiesKilled);

        // Check if a boss is killed
        if (isBoss2Killed)
        {
            Debug.Log("Boss " + boss2Name + " has been killed.");
        }

        // Check if a boss is killed
        if (isBoss3Killed)
        {
            Debug.Log("Boss " + boss3Name + " has been killed.");
        }
    }

    public void IncrementEnemiesKilled()
    {
        enemiesKilled++;
        Debug.Log("Enemies killed: " + enemiesKilled);
    }

    private void DisablePlayerControls()
    {
        // If you're using Unity's Input System
        if (_inputActions != null)
        {
            _inputActions.Disable(); // Disable input actions
        }
    }

    private void UpgradeSprite()
    {
        if (automataChangingSprite != null)
        {
            automataChangingSprite.Upgrade(); // Call the upgrade method from SpriteSwitcher
        }
        else
        {
            Debug.LogWarning("SpriteSwitcher is not assigned to the player.");
        }
    }
}