using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    public GameObject[] slots; // Array of inventory slots
    [SerializeField] private List<Item> itemDatabase; // List of all available items
    public bool[] isfull; // Track which slots are full

    private const char SPLIT_CHAR = '_';
    private string filePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        filePath = Path.Combine(Application.persistentDataPath, "RevitaRogueFinal/Assets/Saves/SaveInventory/Maininventory.txt"); // Use persistent data path
        isfull = new bool[slots.Length]; // Initialize the isfull array
    }

    public void Start()
    {
        LoadInventory();
        //GameEventsManagerSO.instance.miscEvents.GetItemInfo(itemDatabase);
        // Get the item database from the InventoryController
    }
    
    void HandleGetItemInfo(List<string> items)
    {
        // Handle the event
        //Debug.Log("HandleGetItemInfo called with items count: " + items.Count);
    }

    public void SaveInventory()
    {
        // Ensure the directory exists
        string directoryPath = Path.GetDirectoryName(filePath);
        //Debug.Log("Directory path: " + directoryPath); // Debug the directory path

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist
            //Debug.Log("Directory created: " + directoryPath); // Log when the directory is created
        }
        else
        {
            //Debug.Log("Directory already exists: " + directoryPath); // Log if the directory already exists
        }

        using (StreamWriter sw = new StreamWriter(filePath))
        {
            //Debug.Log("Writing to file: " + filePath); // Log when writing starts

            foreach (GameObject slotObject in slots)
            {
                Slot slot = slotObject.GetComponent<Slot>();
                if (slot != null)
                {
                    DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();
                    Spawn spawnComponent = slot.GetComponentInChildren<Spawn>();
                    //Pickup pickupComponent = slot.GetComponentInChildren<Pickup>();

                    if (draggableItem != null && spawnComponent != null  && draggableItem.count > 0)
                    {
                        // Get the item name and count from the draggable item
                        string itemName = spawnComponent.itemName ?? draggableItem.itemCraft.itemName;
                        int itemAmount = draggableItem.count;

                        // Write itemName and amount to the file
                        sw.WriteLine(itemName + SPLIT_CHAR + itemAmount);
                        //Debug.Log("Saved item: " + itemName + " with amount: " + itemAmount); // Log each saved item
                    }
                    else
                    {
                        //Debug.Log("Empty or null slot found."); // Log if the slot is empty or null
                    }
                }
            }
        }

        //Debug.Log("Inventory saved successfully to: " + filePath); // Final log after successful saving
        //Debug.Log(File.ReadAllText(filePath));

    }
    
    public void LoadInventory()
    {
        if (!File.Exists(filePath))
        {
            //Debug.LogWarning("Save file not found, loading empty inventory.");
            return; // No save exists
        }

        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split(SPLIT_CHAR);
                if (parts.Length < 2)
                {
                    //Debug.LogWarning("Invalid save data format, skipping line.");
                    continue; // Skip invalid lines
                }

                string itemName = parts[0]; // The first part is the item name
                if (!int.TryParse(parts[1], out int amount))
                {
                    //Debug.LogWarning($"Invalid amount for item {itemName}, skipping.");
                    continue; // Skip if amount parsing fails
                }

                // Find the item by name instead of ID
                Item item = FindItemByName(itemName);
                if (item != null)
                {
                    // Use the enhanced AddItem method to add the item to the inventory
                    AddItem(item, amount);

                    //Debug.Log($"Loaded Item: {itemName}, Amount: {amount}");
                }
                else
                {
                    //Debug.LogWarning($"Item not found with name: {itemName}, skipping.");
                }
            }
        }

        //Debug.Log("Inventory loaded successfully.");
    }

    public bool HasItem(string itemName)
    {
        foreach (GameObject slotObject in slots)
        {
            Slot slot = slotObject.GetComponent<Slot>();
            DraggableItem draggableItem = slot?.GetComponentInChildren<DraggableItem>();
            Spawn spawnComponent = slot?.GetComponentInChildren<Spawn>();

            if (draggableItem != null && spawnComponent != null && draggableItem.count > 0)
            {
                if (spawnComponent.itemName == itemName)
                {
                    return true; // Item found
                }
            }
        }

        return false; // Item not found
    }
    
    public List<string> GetItemNameDatabase()
    {
        List<string> itemNames = new List<string>();

        foreach (Item item in itemDatabase)
        {
            itemNames.Add(item.itemName);
        }

        return itemNames;
    }

    
    public void RemoveItem(string itemName, int amountToRemove)
    {
        for (int i = 0; i < slots.Length && amountToRemove > 0; i++)
        {
            Slot slot = slots[i].GetComponent<Slot>();
            DraggableItem draggableItem = slot?.GetComponentInChildren<DraggableItem>();
            Spawn spawnComponent = slot?.GetComponentInChildren<Spawn>();

            if (draggableItem != null && spawnComponent != null && draggableItem.count > 0 && spawnComponent.itemName == itemName)
            {
                int itemsToRemove = Mathf.Min(amountToRemove, draggableItem.count);

                draggableItem.count -= itemsToRemove;
                amountToRemove -= itemsToRemove;

                if (draggableItem.count <= 0)
                {
                    Destroy(draggableItem.gameObject);
                    isfull[i] = false;
                }

                draggableItem.RefreshCount();

                if (amountToRemove <= 0)
                {
                    break;
                }
            }
        }
    }
    
    
    public Item FindItemByName(string name)
    {
        foreach (Item item in itemDatabase)
        {
            if (item.itemName == name) // Match by itemName instead of itemID
            {
                return item;
            }
        }
        //Debug.LogWarning("Item not found with name: " + name);
        return null;
    }

    
    public void AddItem(Item newItem, int amountToAdd)
    {
        //Debug.Log("Adding item to inventory: " + newItem.itemName);
        const int STACK_LIMIT = 16;

         // Iterate through the slots to find an available slot or a stackable slot for the same item
        for (int i = 0; i < slots.Length; i++)
        {
            Slot slot = slots[i].GetComponent<Slot>();
            DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();

            // If the slot is empty (i.e., no item is currently assigned)
            if (draggableItem == null || draggableItem.count == 0)
            {
                // Instantiate a new DraggableItem and assign it to the slot
                GameObject newDraggableItemObj = Instantiate(newItem.prefab, slot.transform);
                DraggableItem newDraggableItem = newDraggableItemObj.GetComponent<DraggableItem>();

                // Set the item's sprite and assign the amount
                newDraggableItem.SetItemSprite(newItem.itemSprite);
                newDraggableItem.count = Mathf.Min(amountToAdd, STACK_LIMIT);
                newDraggableItem.RefreshCount();
                newDraggableItem.transform.localPosition = Vector3.zero;

                // Mark the slot as full
                isfull[i] = true;

                // Deduct the added amount from the total to add
                amountToAdd -= newDraggableItem.count;

                if (amountToAdd <= 0) return; // All items added
            }
            else if (draggableItem.itemCraft.itemName == newItem.itemName && draggableItem.count < STACK_LIMIT)
            {
                // Calculate how much can be added to the existing stack without exceeding the limit
                int addableAmount = Mathf.Min(amountToAdd, STACK_LIMIT - draggableItem.count);
                draggableItem.AddToStack(addableAmount);
                draggableItem.RefreshCount();
                amountToAdd -= addableAmount;

                if (amountToAdd <= 0) return; // All items added
            }
        }

        // If there are remaining items after iterating, log a warning or handle as needed
        if (amountToAdd > 0)
        {
            //Debug.LogWarning("Inventory is full or no available slots for remaining items.");
        }
    }


    public void ClearInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Slot slot = slots[i].GetComponent<Slot>();
            DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();

            // If there's an item in the slot, remove it
            if (draggableItem != null)
            {
                Destroy(draggableItem.gameObject); // Remove the draggable item from the slot
                isfull[i] = false; // Mark the slot as empty
            }
        }
        SaveInventory(); 
    }

    
    public Dictionary<string, int> CheckInventoryItems()
    {
        Dictionary<string, int> inventorySummary = new Dictionary<string, int>();

        for (int i = 0; i < slots.Length; i++)
        {
            Slot slot = slots[i].GetComponent<Slot>();
            if (slot != null)
            {
                DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();
                Spawn spawnComponent = slot.GetComponentInChildren<Spawn>();

                // Check if the slot has an item and count is greater than 0
                if (draggableItem != null && spawnComponent != null && draggableItem.count > 0)
                {
                    string itemName = spawnComponent.itemName;
                    int itemCount = draggableItem.count;

                    // If the item is already in the dictionary, sum the count
                    if (inventorySummary.ContainsKey(itemName))
                    {
                        inventorySummary[itemName] += itemCount;
                    }
                    else
                    {
                        inventorySummary[itemName] = itemCount;
                    }

                    
                    //Debug.Log($"Item: {itemName}, Count: {itemCount} in Slot {i}");
                }
            }
        }

        
        //Debug.Log("Current Inventory Summary:");
        foreach (var item in inventorySummary)
        {
            //Debug.Log($"Item: {item.Key}, Total Count: {item.Value}");
        }

        return inventorySummary;
    }

    public Dictionary<string, int> healthItems = new Dictionary<string, int>
    {
        { "Legumes", 1 },
        { "LegumeStew", 3 }
    };

    public Dictionary<string, int> StaminaItems = new Dictionary<string, int>()
    {
        { "Dates", 1 },
        { "DateBar", 3}
    };

    public bool HasHealthItem()
    {
        foreach (GameObject slotObject in slots)
        {
            Slot slot = slotObject.GetComponent<Slot>();
            DraggableItem draggableItem = slot?.GetComponentInChildren<DraggableItem>();
            Spawn spawnComponent = slot?.GetComponentInChildren<Spawn>();

            if (draggableItem != null && spawnComponent != null && draggableItem.count > 0)
            {
                // Check if the item is in the healthItems dictionary
                if (healthItems.ContainsKey(spawnComponent.itemName))
                {
                    return true; // Health item found
                }
            }
        }

        return false; // No health item found
    }

    
    public void RemoveHealthItem(int amountToRemove)
    {
        for (int i = 0; i < slots.Length && amountToRemove > 0; i++)
        {
            Slot slot = slots[i].GetComponent<Slot>();
            DraggableItem draggableItem = slot?.GetComponentInChildren<DraggableItem>();
            Spawn spawnComponent = slot?.GetComponentInChildren<Spawn>();

            if (draggableItem != null && spawnComponent != null && draggableItem.count > 0)
            {
                // Check if the item is in the healthItems dictionary
                if (healthItems.ContainsKey(spawnComponent.itemName))
                {
                    int healthValue = healthItems[spawnComponent.itemName]; // Get health value

                    // Calculate how much health can be removed based on the item count
                    int itemsToRemove = Mathf.Min(amountToRemove / healthValue, draggableItem.count);

                    amountToRemove -= itemsToRemove * healthValue; // Deduct health
                    draggableItem.count -= itemsToRemove; // Deduct item count

                    // If the count drops to 0, destroy the draggable item from the slot
                    if (draggableItem.count <= 0)
                    {
                        Destroy(draggableItem.gameObject);
                        isfull[i] = false; // Mark slot as empty
                    }

                    draggableItem.RefreshCount(); // Update UI count

                    // If the required health has been removed, exit the loop
                    if (amountToRemove <= 0)
                    {
                        break;
                    }
                }
            }
        }

        if (amountToRemove > 0)
        {
            //Debug.LogWarning($"Could not remove the full amount of health items. {amountToRemove} health still needed.");
        }
    }
    
    public bool HasHungerItem()
    {
        foreach (GameObject slotObject in slots)
        {
            Slot slot = slotObject.GetComponent<Slot>();
            DraggableItem draggableItem = slot?.GetComponentInChildren<DraggableItem>();
            Spawn spawnComponent = slot?.GetComponentInChildren<Spawn>();

            if (draggableItem != null && spawnComponent != null && draggableItem.count > 0)
            {
                // Check if the item is in the hungerItems dictionary
                if (StaminaItems.ContainsKey(spawnComponent.itemName))
                {
                    return true; // Hunger item found
                }
            }
        }

        return false; // No hunger item found
    }

    public void RemoveHungerItem(int amountToRemove)
    {
        for (int i = 0; i < slots.Length && amountToRemove > 0; i++)
        {
            Slot slot = slots[i].GetComponent<Slot>();
            DraggableItem draggableItem = slot?.GetComponentInChildren<DraggableItem>();
            Spawn spawnComponent = slot?.GetComponentInChildren<Spawn>();

            if (draggableItem != null && spawnComponent != null && draggableItem.count > 0)
            {
                // Check if the item is in the hungerItems dictionary
                if (StaminaItems.ContainsKey(spawnComponent.itemName))
                {
                    int hungerValue = StaminaItems[spawnComponent.itemName]; // Get hunger value

                    // Calculate how much hunger can be removed based on the item count
                    int itemsToRemove = Mathf.Min(amountToRemove / hungerValue, draggableItem.count);

                    amountToRemove -= itemsToRemove * hungerValue; // Deduct hunger
                    draggableItem.count -= itemsToRemove; // Deduct item count

                    // If the count drops to 0, destroy the draggable item from the slot
                    if (draggableItem.count <= 0)
                    {
                        Destroy(draggableItem.gameObject);
                        isfull[i] = false; // Mark slot as empty
                    }

                    draggableItem.RefreshCount(); // Update UI count

                    // If the required hunger has been removed, exit the loop
                    if (amountToRemove <= 0)
                    {
                        break;
                    }
                }
            }
        }

        if (amountToRemove > 0)
        {
            Debug.LogWarning($"Could not remove the full amount of hunger items. {amountToRemove} hunger still needed.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveInventory();
    }
    
    
}
