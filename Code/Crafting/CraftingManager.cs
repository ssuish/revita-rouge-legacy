using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    private static CraftingManager Instance { get; set; }
    public CraftSlot[] craftSlots; 
    public Button craftButton; 
    

    public List<ItemCraft> itemlist;
    public string[] recipes;
    public ItemCraft[] recipeResults; 
    private InventoryController inventory; 

    private NotificationManager _notificationManager;

    void Awake()
    {
        _notificationManager = FindObjectOfType<NotificationManager>(); 

        // Singleton implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        
        craftButton.onClick.AddListener(CraftItem);

        // Find the inventory in the scene
        inventory = FindObjectOfType<InventoryController>();
        
    }

    private void ClearCraftSlots()
    {
        foreach (CraftSlot slot in craftSlots)
        {
            slot.ClearSlot(); // Call ClearSlot() on each CraftSlot
        }
    }

    private void CraftItem()
    {
        // Check if the inventory has space
        if (IsInventoryFull())
        {
            RestoreItemsToInventory();
            ClearCraftSlots();
            //_notificationManager?.ShowNotification("Inventory is full! Cannot craft. Returning items.", 0);
            return;
        }

        // Find matching recipe
        int recipeIndex = FindMatchingRecipe();

        if (recipeIndex == -1)
        {
            // No matching recipe found
            RestoreItemsToInventory();
            ClearCraftSlots();
            //_notificationManager?.ShowNotification("No matching recipe found. Returning items.", 0);
            return;
        }

        // Check if the crafting requirements are met
        if (!AreCraftingRequirementsMetForRecipe(recipeIndex))
        {
            RestoreItemsToInventory();
            ClearCraftSlots();
            //_notificationManager?.ShowNotification("Crafting requirements not met. Returning items.", 0);
            return;
        }

        // Deduct required items
        DeductRequiredItemsFromCraftSlots(recipeIndex);

        // Handle excess items
        HandleExcessItems();

        // Craft the item
        ItemCraft craftedItem = recipeResults[recipeIndex];
        bool itemAddedToInventory = AddItemToInventory(craftedItem);

        if (itemAddedToInventory)
        {
            //_notificationManager?.ShowNotification($"Crafted {craftedItem.itemName} successfully!", 0);
            GameEventsManagerSO.instance.itemEvents.ItemCrafted(craftedItem.itemName, true);
            ClearCraftSlots();
        }
        else
        {
            // If crafted item cannot be added, restore the items
            RestoreItemsToInventory();
            ClearCraftSlots();
            //_notificationManager?.ShowNotification($"Not enough space for crafted item {craftedItem.itemName}. Returning items.", 0);
        }
        // Deduct required items
        DeductRequiredItemsFromCraftSlots(recipeIndex);

        // Handle excess items
        HandleExcessItems();
    }
    
    public void RestoreItemsToInventory()
    {
        foreach (CraftSlot slot in craftSlots)
        {
            if (slot.itemCraft != null)
            {
                AddItemToInventory(slot.itemCraft);
                slot.RestoreDraggableItem();
            }
        }
    }


    private void HandleExcessItems()
    {
        //Debug.Log("HandleExcessItems called.");

        // Dictionary to store the required items and their amounts based on the recipe
        Dictionary<string, int> requiredItemCounts = new Dictionary<string, int>();

        // First, gather the recipe requirements from the matched recipe
        int recipeIndex = FindMatchingRecipe();
        if (recipeIndex != -1)
        {
            string[] requiredItems = recipes[recipeIndex].Split(' ');
            foreach (string itemEntry in requiredItems)
            {
                if (string.IsNullOrWhiteSpace(itemEntry) || itemEntry.ToLower() == "null")
                {
                    continue; // Skip null or empty recipe items
                }

                string[] itemParts = itemEntry.Split(':');
                string itemName = itemParts[0];
                int requiredAmount = int.Parse(itemParts[1]);

                requiredItemCounts[itemName] = requiredAmount;
            }
        }

        // Now, iterate through the craft slots and handle excess items
        foreach (CraftSlot slot in craftSlots)
        {
            if (slot.itemCraft != null && slot.currentDraggableItem != null)
            {
                string itemName = slot.itemCraft.itemName;
                int placedCount = slot.currentDraggableItem.count;

                if (requiredItemCounts.TryGetValue(itemName, out var requiredCount))
                {
                    //Debug.Log($"Required count for {itemName}: {requiredCount}, Placed count: {placedCount}");

                    // Calculate excess amount
                    int excessAmount = placedCount - requiredCount;
                    if (excessAmount > 0)
                    {
                        //Debug.Log($"Excess amount for {itemName}: {excessAmount}");
                        AddExcessToInventory(slot.itemCraft, excessAmount); // Add excess items to the inventory
                    }
                    
                }
            }
        }

        //Debug.Log("Clearing craft slots after handling excess items.");
    }


    private int FindMatchingRecipe()
    {
        for (int i = 0; i < recipes.Length; i++)
        {
            // Check if the current recipe's requirements are met
            if (AreCraftingRequirementsMetForRecipe(i))
            {
                //Debug.Log($"Matching recipe found: {recipes[i]} for crafting item {recipeResults[i].itemName}");
                return i; // Return the index of the matching recipe
            }
        }
        
        //Debug.Log("No matching recipe found.");
        return -1; // No matching recipe found
        
    }

    private bool AreCraftingRequirementsMetForRecipe(int recipeIndex)
    {
        // Get the recipe requirements for the given index
        string[] requiredItems = recipes[recipeIndex].Split(' '); // Assuming space-separated item:quantity pairs

        if (requiredItems.Length == 0)
        {
            // Debug.LogError($"Recipe at index {recipeIndex} is empty or invalid.");
            return false;
        }

        // Create a dictionary to hold the item names and their quantities in crafting slots
        Dictionary<string, int> itemsInSlots = new Dictionary<string, int>();

        foreach (CraftSlot slot in craftSlots)
        {
            if (slot.itemCraft != null && slot.currentDraggableItem != null)
            {
                string itemName = slot.itemCraft.itemName;
                int itemCount = int.Parse(slot.countText.text); // Get the count from the text

                if (!itemsInSlots.TryAdd(itemName, itemCount))
                {
                    itemsInSlots[itemName] += itemCount; // Add the quantity if the item already exists
                }
            }
        }

        // Iterate through the required items and their quantities
        foreach (string requiredItemEntry in requiredItems)
        {
            if (string.IsNullOrWhiteSpace(requiredItemEntry) || requiredItemEntry.ToLower() == "null")
            {
                continue; // Skip null or empty recipe items
            }

            string[] itemParts = requiredItemEntry.Split(':');
            if (itemParts.Length != 2)
            {
                // Debug.LogError($"Invalid recipe format at index {recipeIndex}: {requiredItemEntry}");
                return false;
            }

            var requiredItem = itemParts[0];
            if (!int.TryParse(itemParts[1], out var requiredQuantity))
            {
                foreach (CraftSlot slot in craftSlots)
                {
                    slot.RestoreDraggableItem();
                }

                return false;
            }

            if (!itemsInSlots.ContainsKey(requiredItem) || itemsInSlots[requiredItem] < requiredQuantity)
            {
                foreach (CraftSlot slot in craftSlots)
                {
                    slot.RestoreDraggableItem();
                }

                return false;
            }

            itemsInSlots[requiredItem] -= requiredQuantity;
            if (itemsInSlots[requiredItem] <= 0)
            {
                itemsInSlots.Remove(requiredItem);
            }
        }

        return true; // All requirements are met, and inventory has space

    }
    private bool IsInventoryFull()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            Slot slot = inventory.slots[i].GetComponent<Slot>();
            DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();

            if (draggableItem == null)
            {
                //Debug.Log($"Slot {i} is empty.");
                return false; // Found an empty slot
            }
            else
            {
                //Debug.Log($"Slot {i} is occupied by {draggableItem.name}, count: {draggableItem.count}.");
            }
        }
        //Debug.Log("All slots are full.");
        return true; // All slots are full
    }


    private bool AddItemToInventory(ItemCraft craftedItem)
    {
        bool itemAdded = false;

        // Check if the item can be stacked in any slot
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isfull[i])
            {
                Slot slot = inventory.slots[i].GetComponent<Slot>();
                DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();
                Spawn spawnComponent = slot.GetComponentInChildren<Spawn>();

                if (draggableItem != null && spawnComponent != null)
                {
                    // Check if the item can be stacked
                    if (draggableItem.count < 16 && spawnComponent.itemName == craftedItem.itemName)
                    {
                        // Stack the item and update the UI
                        draggableItem.AddToStack(1);
                        draggableItem.transform.localPosition =
                            Vector3.zero; // Position the item in the center of the slot
                        itemAdded = true;
                        break;
                    }
                }
            }
        }

        // If the item cannot be stacked, find an empty slot
        if (!itemAdded)
        {
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (!inventory.isfull[i])
                {
                    // Found an empty slot, adding the item
                    inventory.isfull[i] = true;
                    GameObject newItem =
                        Instantiate(craftedItem.itemButtonPrefab, inventory.slots[i].transform,
                            false); // Use the specific prefab
                    DraggableItem draggableItem = newItem.GetComponent<DraggableItem>();
                    Spawn spawnComponent = newItem.GetComponentInChildren<Spawn>();

                    if (draggableItem != null && spawnComponent != null)
                    {
                        // Initialize the new item in the inventory slot
                        if (draggableItem.count <= 0)
                        {
                            draggableItem.count = 1; // Set the count to 1 if it is not already set
                        } // Set the count to 1 for the newly crafted item

                        draggableItem.RefreshCount();
                        spawnComponent.itemName = craftedItem.itemName; // Set the item name
                        draggableItem.transform.localPosition =
                            Vector3.zero; // Position the item in the center of the slot
                        // Force the item to be positioned at (0, 0, 0) within the slot
                        //newItem.transform.localPosition = Vector3.zero; // Or new Vector3(0, 0, 0) to be explicit

                        itemAdded = true;
                        break;
                    }
                }
            }
        }

        return itemAdded;
    }

    private void DeductRequiredItemsFromCraftSlots(int recipeIndex)
    {
        //Debug.Log("DeductRequiredItemsFromCraftSlots called");

        string[] requiredItems = recipes[recipeIndex].Split(' ');
        Dictionary<string, int> requiredItemCounts = new Dictionary<string, int>();

        // Count how many of each item and amount is required
        foreach (string item in requiredItems)
        {
            if (string.IsNullOrWhiteSpace(item) || item.ToLower() == "null")
            {
                continue; // Skip null or empty items in the recipe
            }

            string[] itemInfo = item.Split(':');
            string itemName = itemInfo[0];
            int requiredAmount = int.Parse(itemInfo[1]);

            if (!requiredItemCounts.TryAdd(itemName, requiredAmount))
            {
                requiredItemCounts[itemName] += requiredAmount;
            }
        }

        // Deduct the required items from the craft slots
        foreach (CraftSlot slot in craftSlots)
        {
            if (slot.itemCraft != null && requiredItemCounts.ContainsKey(slot.itemCraft.itemName))
            {
                //Debug.Log($"Checking slot with item: {slot.itemCraft.itemName}");

                DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();
                if (draggableItem != null)
                {
                    int requiredAmount = requiredItemCounts[slot.itemCraft.itemName];
                    int amountToDeduct = Mathf.Min(requiredAmount, draggableItem.count);

                    //Debug.Log($"Deducting {amountToDeduct} of {slot.itemCraft.itemName} from slot. Current count: {draggableItem.count}");

                    // Deduct the required amount
                    draggableItem.count -= amountToDeduct;
                    draggableItem.RefreshCount();

                    // Update the required item count
                    requiredItemCounts[slot.itemCraft.itemName] -= amountToDeduct;
                    if (requiredItemCounts[slot.itemCraft.itemName] <= 0)
                    {
                        requiredItemCounts.Remove(slot.itemCraft.itemName);
                    }
                }
            }
        }
    }

    private void AddExcessToInventory(ItemCraft excessItem, int excessCount)
    {
        //Debug.Log($"Attempting to add {excessCount} excess {excessItem.itemName} back to the inventory.");
        bool excessAdded = false;

        //excessItem.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        //excessItem.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;

        // Try to add the excess items to an existing stack
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isfull[i])
            {
                Slot slot = inventory.slots[i].GetComponent<Slot>();
                DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();
                Spawn spawnComponent = slot.GetComponentInChildren<Spawn>();

                if (draggableItem != null && spawnComponent != null)
                {
                    // Check if the item can be stacked
                    if (draggableItem.count < 16 && spawnComponent.itemName == excessItem.itemName)
                    {
                        // Calculate how many items can be stacked in this slot
                        int stackableAmount = Mathf.Min(16 - draggableItem.count, excessCount);
                        draggableItem.AddToStack(stackableAmount);
                        excessCount -= stackableAmount;

                        //Debug.Log($"Added {stackableAmount} excess {excessItem.itemName} to existing stack in slot {i}. Remaining excess: {excessCount}");

                        if (excessCount <= 0)
                        {
                            //Debug.Log("All excess items added to inventory.");
                            excessAdded = true;
                            break;
                        }
                    }
                }
            }
        }

        // If the excess items cannot be stacked, find empty slots for them
        if (excessCount > 0)
        {
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (!inventory.isfull[i])
                {
                    // Found an empty slot, adding the excess items
                    inventory.isfull[i] = true;
                    GameObject newItem = Instantiate(excessItem.itemButtonPrefab, inventory.slots[i].transform, false); // Use the specific prefab
                    newItem.SetActive(true); // Ensure the item is visible

                    DraggableItem draggableItem = newItem.GetComponent<DraggableItem>();
                    Spawn spawnComponent = newItem.GetComponentInChildren<Spawn>();

                    if (draggableItem == null)
                    {
                        // Ensure the item is draggable
                        draggableItem = newItem.AddComponent<DraggableItem>();
                    }

                    if (draggableItem != null && spawnComponent != null)
                    {
                        // Initialize the new item in the inventory slot
                        draggableItem.count = excessCount; // Set the count to remaining excess
                        draggableItem.RefreshCount();
                        spawnComponent.itemName = excessItem.itemName; // Set the item name

                        newItem.transform.localPosition = Vector3.zero; // Position the item in the center of the slot

                        //Debug.Log($"Placed {excessCount} excess {excessItem.itemName} into empty inventory slot {i}.");
                        excessAdded = true;
                        break;
                    }
                }
            }
        }

        // If inventory is full and there are still excess items left
        if (!excessAdded && excessCount > 0)
        {
            _notificationManager.ShowNotification($"Inventory is full, Discarding {excessItem.itemName} ",excessCount);
            //Debug.LogWarning($"Cannot add {excessCount} excess items of {excessItem.itemName} to inventory. Inventory is full.");
        }
    }
}