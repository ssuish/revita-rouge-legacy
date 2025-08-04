using System.Collections;
using UnityEngine;

public class SeituneTreeBehave : MonoBehaviour
{
    [SerializeField] private Animator treeAnimator; 
    [SerializeField] private GameObject itemPrefab; 
    [SerializeField] private Transform dropPoint; 
    [SerializeField] private Transform player; 
    [SerializeField] private float detectionRadius = 3f; 
    [SerializeField] private float animationSpeed = 0.5f;
    [SerializeField] private string requiredItem = "SeituneSeed"; 

    private bool playerIsNear = false; 
    private bool treeIsBlooming = false; 
    private bool treeIsIdle = true; 
    private bool isTriggered = false; 

    // Subscribe to the boss's death event
    private void OnEnable()
    {
        Boss.OnBossDefeated += TriggerTreeAnimation;
    }

    private void OnDisable()
    {
        Boss.OnBossDefeated -= TriggerTreeAnimation;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // If the player is within detection radius and the tree is idle
        if (distance <= detectionRadius && !playerIsNear && treeIsIdle)
        {
            if (HasItemInInventory(requiredItem)) // Check for SeituneSeed in inventory
            {
                StartBlooming();
                RemoveItemFromInventory(requiredItem); // Remove the seed after blooming starts
            }
            
        }
        // If the player leaves the detection radius and the tree is blooming
        else if (distance > detectionRadius && playerIsNear && treeIsBlooming)
        {
            StopBlooming();
        }
    }

    // Trigger the animation when the boss is defeated
    private void TriggerTreeAnimation()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            StartBlooming(); // Start blooming when the boss is defeated
        }
    }

    // Trigger tree bloom animation when the player is near
    private void StartBlooming()
    {
        treeAnimator.speed = animationSpeed; // Slow down the animation speed
        treeAnimator.SetBool("treeIsBlooming", true); // Set the blooming animation
        treeIsIdle = false;  // Tree is no longer idle
        playerIsNear = true; // Player is near the tree

        // Start a coroutine to wait for the animation completion
        StartCoroutine(WaitForBloomingToComplete());
    }

    // Trigger tree to return to idle when the player leaves
    private void StopBlooming()
    {
        treeAnimator.SetBool("treeIsBlooming", false); // Set the tree back to idle animation
        treeIsBlooming = false; // Tree is no longer blooming
        playerIsNear = false; // Player is no longer near

        // Start a coroutine to wait for the animation completion
        StartCoroutine(WaitForIdleToComplete());
    }

    // Coroutine to wait for the blooming animation to complete
    IEnumerator WaitForBloomingToComplete()
    {
        // Wait for the animator to enter the "Bloomin" state
        while (!treeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Bloomin"))
        {
            yield return null; // Wait for next frame until the animation starts
        }

        AnimatorStateInfo stateInfo = treeAnimator.GetCurrentAnimatorStateInfo(0);

        // Wait until the "Bloomin" animation completes
        while (stateInfo.IsName("Bloomin") && stateInfo.normalizedTime < 1f)
        {
            yield return null; // Wait for next frame
            stateInfo = treeAnimator.GetCurrentAnimatorStateInfo(0); // Update state info
        }

        treeIsBlooming = true; // Mark the tree as fully blooming

        // Drop the item after the blooming animation completes
        DropItem();
    }

    // Coroutine to wait for the idle animation to complete
    IEnumerator WaitForIdleToComplete()
    {
        // Wait for the animator to enter the "Idle" state
        while (!treeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null; // Wait for next frame until the animation starts
        }

        AnimatorStateInfo stateInfo = treeAnimator.GetCurrentAnimatorStateInfo(0);

        // Continuously check if the animation is close to finishing
        while (stateInfo.IsName("Idle") && stateInfo.normalizedTime < 1f)
        {
            yield return null; // Wait for next frame
            stateInfo = treeAnimator.GetCurrentAnimatorStateInfo(0); // Update state info
        }

        treeIsIdle = true; // Mark the tree as idle
    }

    // Drop the item from the tree (called after blooming animation)
    public void DropItem()
    {
        Instantiate(itemPrefab, dropPoint.position, Quaternion.identity); // Drop the item at the specified point
    }

    // Check if the player has the required item (SeituneSeed) in the inventory
    private bool HasItemInInventory(string item)
    {
        foreach (GameObject slotObject in InventoryController.Instance.slots)
        {
            Slot slot = slotObject.GetComponent<Slot>();
            DraggableItem draggableItem = slot?.GetComponentInChildren<DraggableItem>();
            Spawn spawnComponent = slot?.GetComponentInChildren<Spawn>();

            // Check if the slot has an item
            if (draggableItem != null && spawnComponent != null && draggableItem.count > 0)
            {
                // Check if the item matches the required item
                if (spawnComponent.itemName == item)
                {
                    return true; // Required item found
                }
            }
        }

        return false; // Required item not found
    }

    // Remove the required item (SeituneSeed) from the inventory
    private void RemoveItemFromInventory(string item)
    {
        for (int i = 0; i < InventoryController.Instance.slots.Length; i++)
        {
            Slot slot = InventoryController.Instance.slots[i].GetComponent<Slot>();
            DraggableItem draggableItem = slot?.GetComponentInChildren<DraggableItem>();
            Spawn spawnComponent = slot?.GetComponentInChildren<Spawn>();

            if (draggableItem != null && spawnComponent != null && draggableItem.count > 0)
            {
                // Check if the item matches the required item
                if (spawnComponent.itemName == item)
                {
                    draggableItem.count--; // Decrease the item count

                    // If the count drops to 0, destroy the draggable item from the slot
                    if (draggableItem.count <= 0)
                    {
                        Destroy(draggableItem.gameObject);
                        InventoryController.Instance.isfull[i] = false; // Mark slot as empty
                    }

                    draggableItem.RefreshCount(); // Update UI count
                    break; // Exit once the item is removed
                }
            }
        }
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
