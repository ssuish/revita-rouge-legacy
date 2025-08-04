using System.Collections;
using UnityEngine;

public class ChangeSpriteOnCollision : MonoBehaviour
{
    [SerializeField] private Sprite[] dumpsCollision;  // Array of sprites for depleted states
    [SerializeField] private Sprite emptySprite;       // Sprite when the resource is completely depleted
    [SerializeField] private Sprite filledSprite;      // Sprite when the resource is replenished
    [SerializeField] private float replenishTime = 5f; // Time in seconds to replenish the resource
    [SerializeField] private float gatheringTime = 3f; // Time required to gather the resource

    private SpriteRenderer spriteRenderer;
    private bool isDepleted = false;
    private int currentIndex = 0; // Tracks the current index in the sprite array
    private Coroutine replenishCoroutine;
    private bool isGathering = false;
    private float gatheringTimer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if the required sprites are assigned
        if (dumpsCollision.Length == 0)
        {
            Debug.LogError("DumpsCollision array is empty. Please assign sprites in the Inspector.");
        }

        gatheringTimer = gatheringTime; // Initialize the gathering timer
    }

    private void Update()
    {
        // Check if the player is gathering
        if (isGathering && !isDepleted)
        {
            gatheringTimer -= Time.deltaTime; // Decrease the gathering timer
            if (gatheringTimer <= 0f)
            {
                GatherResource(); // Call the gather logic when the timer is depleted
                gatheringTimer = gatheringTime; // Reset the timer for the next gathering attempt
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isDepleted && !isGathering)
        {
            // Start gathering when colliding with the resource
            isGathering = true;
            gatheringTimer = gatheringTime; // Reset the timer
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // Stop gathering when leaving the resource
        isGathering = false;
    }

    private void GatherResource()
    {
        if (!isDepleted)
        {
            // Increment or loop back the index
            currentIndex = (currentIndex + 1) % dumpsCollision.Length;

            // Set the sprite to the current one in the array
            spriteRenderer.sprite = dumpsCollision[currentIndex];

            // If this was the last sprite in the array, switch to the empty sprite
            if (currentIndex == dumpsCollision.Length - 1)
            {
                StartCoroutine(HandleEmptyState()); // Start the process to display the empty sprite
            }
            else
            {
                // Only start the replenish coroutine if it's not already running
                if (replenishCoroutine != null)
                {
                    StopCoroutine(replenishCoroutine);
                }
                replenishCoroutine = StartCoroutine(ReplenishResource()); // Start the replenish timer for non-final states
            }

            // Stop gathering after one successful gather
            isGathering = false;
        }
    }

    private IEnumerator HandleEmptyState()
    {
        yield return new WaitForSeconds(0.1f); 
        spriteRenderer.sprite = emptySprite; // Change to the empty sprite
        isDepleted = true;

        // Only start the replenish coroutine if it's not already running
        if (replenishCoroutine != null)
        {
            StopCoroutine(replenishCoroutine);
        }
        replenishCoroutine = StartCoroutine(ReplenishResource()); // Start the replenish timer
    }

    private IEnumerator ReplenishResource()
    {
        // Wait for the specified time before replenishing
        yield return new WaitForSeconds(replenishTime);

        // Change the sprite back to the filled state
        spriteRenderer.sprite = filledSprite;
        isDepleted = false;
        currentIndex = 0; // Reset the index for the next cycle
    }

    // This method can be called by your input manager when the gather button is pressed
    public void StartGathering()
    {
        if (!isDepleted && !isGathering)
        {
            isGathering = true;
            gatheringTimer = gatheringTime; // Reset the timer when starting to gather
        }
    }

    public void StopGathering()
    {
        isGathering = false;
        gatheringTimer = gatheringTime; // Reset the timer when stopping to gather
    }
}
