using System.Collections;
using UnityEngine;

public class LegumesInteractions : MonoBehaviour
{
    public Sprite depletedSprite;
    public Sprite filledSprite;
    public Sprite selectedSprite;
    public Sprite secondInteractionSprite;

    private SpriteRenderer spriteRenderer;
    private Coroutine refillCoroutine;
    private bool hasInteractedOnce = false;

    private float refillDelay = 1200f;
    public float interactionTime = 2f; // Time required to interact with the dump

    private PlayerActionMap _inputActions;
    private bool isInteracting = false;
    private float interactionTimer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = filledSprite; // Start with filled sprite
        interactionTimer = interactionTime; // Initialize the timer
    }

    private void Awake()
    {
        _inputActions = new PlayerActionMap();
        _inputActions.Player.Enable();
        
    }

    void Update()
    {
        if (isInteracting)
        {
            if (_inputActions.Player.Gathering.ReadValue<float>() > 0)
            {
                interactionTimer -= Time.deltaTime; // Decrease timer

                if (interactionTimer <= 0)
                {
                    HandleInteractionLegumes(); // Handle the interaction
                }
            }
            else
            {
                interactionTimer = interactionTime; // Reset timer if not holding the button
            }
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInteracting = true; // Player is interacting
            spriteRenderer.sprite = hasInteractedOnce ? secondInteractionSprite : selectedSprite; // Change sprite based on interaction
            interactionTimer = interactionTime; // Reset the timer when entering
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInteracting = false; // Stop tracking interaction
            interactionTimer = interactionTime; // Reset timer

            // Change sprite based on interaction state
            if (hasInteractedOnce)
            {
                spriteRenderer.sprite = depletedSprite; // If interacted, show depleted
            }
            else
            {
                spriteRenderer.sprite = filledSprite; // Else, show filled
            }
        }
    }

    public void HandleInteractionLegumes()
    {
        if (!hasInteractedOnce)
        {
            spriteRenderer.sprite = depletedSprite; // Set to depleted
            hasInteractedOnce = true; // Mark as interacted

            // Start refill coroutine
            if (refillCoroutine != null)
            {
                StopCoroutine(refillCoroutine);
            }
            refillCoroutine = StartCoroutine(RefillAfterDelay());
        }
    }

    IEnumerator RefillAfterDelay()
    {
        yield return new WaitForSeconds(refillDelay);
        RefillDump();
    }

    public void RefillDump()
    {
        spriteRenderer.sprite = filledSprite; // Reset to filled
        hasInteractedOnce = false; // Reset interaction state
    }
}
