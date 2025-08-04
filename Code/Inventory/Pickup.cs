using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private InventoryController inventory;
    public GameObject itemButton;
    public string itemName;
    public ItemSpawner spawner;
    private DatesSpawner datesSpawner;
    private LegumesSpawner legumesSpawner;
    private NotificationManager notificationManager; // Reference to the notification manager

    
    private void Start()
    {
        inventory = FindObjectOfType<InventoryController>();
        notificationManager = FindObjectOfType<NotificationManager>(); // Get the NotificationManager in the scene

        if (inventory == null)
        {
            //Debug.LogError("InventoryController not found in the scene.");
        }

        if (itemButton == null)
        {
            //Debug.LogError("ItemButton is not assigned.");
        }

        if (spawner == null)
        {
            //Debug.LogError("Spawner is not assigned.");
        }
    }

    /*public void PickupItem()
    {
        bool itemPickedUp = false;
        DraggableItem draggableItem = null;
        Spawn spawnComponent = null;
        int totalPickedUp = 0;
        string pickedUpItemName = itemName;

        // First loop: Try to add item to existing stacks
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isfull[i])
            {
                Slot slot = inventory.slots[i].GetComponent<Slot>();
                if (slot != null)
                {
                    draggableItem = slot.GetComponentInChildren<DraggableItem>();
                    spawnComponent = slot.GetComponentInChildren<Spawn>();

                    if (draggableItem != null && spawnComponent != null && spawnComponent.itemName == itemName)
                    {
                        int spaceLeft = 16 - draggableItem.count;
                        int amountToAdd = Mathf.Min(spaceLeft, 1);
                        draggableItem.AddToStack(amountToAdd);
                        totalPickedUp += amountToAdd;
                        itemPickedUp = true;
                    }
                }
            }
        }

        // Second loop: Place item in an empty slot if no existing stack found
        if (!itemPickedUp)
        {
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (!inventory.isfull[i])
                {
                    Slot slot = inventory.slots[i].GetComponent<Slot>();
                    GameObject newItem = Instantiate(itemButton, inventory.slots[i].transform, false);

                    draggableItem = newItem.GetComponent<DraggableItem>();
                    spawnComponent = newItem.GetComponentInChildren<Spawn>();

                    if (draggableItem != null && spawnComponent != null)
                    {
                        draggableItem.count = 1;
                        totalPickedUp += 1;
                        draggableItem.RefreshCount();
                        slot.PickUpItem(draggableItem);

                        // Mark this slot as full only after placing the item
                        inventory.isfull[i] = true;
                        itemPickedUp = true;
                        break; // Exit the loop once item is placed in an empty slot
                    }
                }
            }
        }

        // Notify total picked up only once
        if (totalPickedUp > 0)
        {
            notificationManager.ShowNotification(pickedUpItemName, totalPickedUp);
            GameEventsManagerSO.instance.itemEvents.ItemCollectedWithCount(pickedUpItemName, totalPickedUp);
            Destroy(gameObject);

            if (spawner != null)
            {
                spawner.StartRespawnCoroutine(transform.position);
            }
        }
        
        // Check if inventory is full after attempting to pick up item
        if (!itemPickedUp)
        {
            notificationManager.ShowNotification("Inventory Full", 0);
        }
    }*/
    public void PickupItem()
    {
        bool itemAddedToExistingStack = false;
        int totalPickedUp = 0;
        string pickedUpItemName = itemName;

        // First loop: Add item to existing stacks
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isfull[i])
            {
                Slot slot = inventory.slots[i].GetComponent<Slot>();
                DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();
                Spawn spawnComponent = slot.GetComponentInChildren<Spawn>();

                if (draggableItem != null && spawnComponent != null && spawnComponent.itemName == itemName)
                {
                    int spaceLeft = 16 - draggableItem.count;
                    if (spaceLeft > 0)
                    {
                        int amountToAdd = Mathf.Min(spaceLeft, 1);
                        draggableItem.AddToStack(amountToAdd);
                        totalPickedUp += amountToAdd;
                        itemAddedToExistingStack = true;

                        if (draggableItem.count == 16)
                        {
                            // If stack limit is reached, skip setting itemPickedUp and continue to new stack
                            continue;
                        }
                    }
                }
            }
        }

        // Second loop: Add new stack if no existing stack or stack limit reached
        if (!itemAddedToExistingStack || totalPickedUp < 1)
        {
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (!inventory.isfull[i])
                {
                    Slot slot = inventory.slots[i].GetComponent<Slot>();
                    GameObject newItem = Instantiate(itemButton, inventory.slots[i].transform, false);
                    DraggableItem draggableItem = newItem.GetComponent<DraggableItem>();
                    Spawn spawnComponent = newItem.GetComponentInChildren<Spawn>();

                    if (draggableItem != null && spawnComponent != null)
                    {
                        draggableItem.count = 1;
                        totalPickedUp += 1;
                        draggableItem.RefreshCount();
                        slot.PickUpItem(draggableItem);
                        inventory.isfull[i] = true;
                        break;
                    }
                }
            }
        }

        if (totalPickedUp > 0)
        {
            notificationManager.ShowNotification(pickedUpItemName, totalPickedUp);
            GameEventsManagerSO.instance.itemEvents.ItemCollectedWithCount(pickedUpItemName, totalPickedUp);
            Destroy(gameObject);
        
            if (spawner != null)
            {
                spawner.StartRespawnCoroutine(transform.position);
            }
        }

        // Notify if inventory full
        if (totalPickedUp == 0)
        {
            notificationManager.ShowNotification("Inventory Full", 0);
        }
    }

}