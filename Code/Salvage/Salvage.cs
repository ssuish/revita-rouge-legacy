using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class Salvage : MonoBehaviour
{
    public static Salvage Instance { get; private set; }
    public SalvageSlot salvageSlot;
    public Button salvageButton;
    public List<ItemSalvage> salvageableItems; // List of items that can be salvaged
    public List<ItemSalvage> SalvageResults;   // List of items that can be returned after salvaging
    private InventoryController inventory;
    public Leveling LevelingSystem;

    private int salvageUses = 0; // Count of salvage uses
    private const int maxSalvageUses = 8; // Maximum number of uses allowed
    private DateTime lastSalvageTime; // Last time salvage was performed

    // TimeSpan changed to seconds for testing purposes (e.g., 30 seconds cooldown)
    private TimeSpan cooldownPeriod = TimeSpan.FromSeconds(30); // Change this to any number of seconds for testing

    public TextMeshProUGUI cooldowntext;

    void Awake()
    {
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
        salvageButton.onClick.AddListener(SalvageItem);
        inventory = FindObjectOfType<InventoryController>();
        

        // Load saved data for salvage uses and last salvage time
        LoadSalvageData();
        
        cooldowntext.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        // Check if the player has insufficient EXP
        if (LevelingSystem.CurrentXp <= 0)
        {
            cooldowntext.text = "Insufficient EXP. Gain some EXP before salvaging!";
            cooldowntext.gameObject.SetActive(true);
            return;
        }
        // Check if the player is in cooldown and update the text
        if (salvageUses >= maxSalvageUses)
        {
            TimeSpan timeRemaining = cooldownPeriod - (DateTime.Now - lastSalvageTime);
            
            if (timeRemaining.TotalSeconds > 0)
            {
                // Display remaining cooldown time in seconds
                cooldowntext.text = "Cooldown: " + timeRemaining.Seconds + "s remaining";
                cooldowntext.gameObject.SetActive(true);
            }
            else
            {
                // Cooldown finished, allow salvaging again
                salvageUses = 0;
                cooldowntext.gameObject.SetActive(false);
            }
        }
    }

    public void SalvageItem()
    {
        if (LevelingSystem.CurrentXp <= 0)
        {
            //Debug.LogWarning("Not enough experience to perform salvage.");
            cooldowntext.text = "Insufficient EXP. Gain some EXP before salvaging!";
            cooldowntext.gameObject.SetActive(true);
            return;
        }
        // Check if the player can salvage based on uses and cooldown
        if (CanSalvage())
        {
            if (salvageSlot != null && salvageSlot.itemSalvage != null)
            {
                // Declare a flag to indicate if the item should be returned
                bool returnToOriginalSlot;
                // Check if the item in the slot is in the salvageableItems list
                if (IsItemSalvageable(salvageSlot.itemSalvage, out returnToOriginalSlot))
                {
                    // Pick a random item from the SalvageResults list
                    ItemSalvage randomItem = SalvageResults[Random.Range(0, SalvageResults.Count)];
                    //Debug.Log("Salvaging item and adding random result: " + randomItem.itemNameSalvage);

                    // Use the return value of AddSalvagedItemToInventory to check if the item was added
                    bool itemAdded = AddSalvagedItemToInventory(randomItem);

                    if (itemAdded)
                    {
                        // Add experience points
                        int randomExp = Random.Range(2,4);
                        //Debug.Log("Adding EXP: " + randomExp);
                        LevelingSystem.CurrentXp -= randomExp;
                        LevelingSystem.ExperienceController();

                        // Clear the salvage slot after successfully salvaging the item
                        salvageSlot.ClearSlot();

                        // Increment salvage uses and update the last salvage time
                        salvageUses++;
                        lastSalvageTime = DateTime.Now;
                        SaveSalvageData();

                        
                    }
                    
                }
                else
                {
                    // If item is not salvageable, check if it should be returned
                    if (returnToOriginalSlot)
                    {
                        //Debug.LogWarning("Item is exempt from salvage and has been returned to the original slot.");
                        salvageSlot.ReturnToOriginalSlot();
                    }
                    
                }
            }
        }
        
    }

    // Method to check if the player can salvage (checks uses and cooldown)
    private bool CanSalvage()
    {
        // Check if max uses have been reached
        if (salvageUses >= maxSalvageUses)
        {
            // Check if the cooldown period has passed
            if (DateTime.Now - lastSalvageTime >= cooldownPeriod)
            {
                // Reset the salvage uses and allow salvaging again
                salvageUses = 0;
                return true;
            }
        }

        return true; // Allow salvage if uses are below the limit
    }

    private List<string> exemptedItems = new List<string>
    {
        "SunstoneCrystal",
        "RainwaterHarvester",
        "FuelCellElectricEngine",
        "FuelCell",
        "SolarPanel",
        "MistyLeaves",
        "SeituneSeed"
    };
    private bool IsItemSalvageable(ItemSalvage item, out bool returnToOriginalSlot)
    {
        // Check if the item is in the exempted items list
        if (exemptedItems.Contains(item.itemNameSalvage))
        {
            Debug.LogWarning(item.itemNameSalvage + " cannot be salvaged. Returning to the original slot.");
            returnToOriginalSlot = true;
            return false;
        }

        // Allow any item not in the exempted list to be salvaged
        returnToOriginalSlot = false;
        return true;
    }

    public bool AddSalvagedItemToInventory(ItemSalvage salvagedItem)
    {
        bool itemAdded = false;

        int randomCount = Random.Range(1, 5);

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
                    if (draggableItem.count < 16 && spawnComponent.itemName == salvagedItem.itemNameSalvage)
                    {
                        // Add as much of the random count as possible to the stack
                        int spaceAvailable = 16 - draggableItem.count;
                        int amountToAdd = Mathf.Min(randomCount, spaceAvailable);
                        draggableItem.AddToStack(amountToAdd);

                        randomCount -= amountToAdd;

                        if (randomCount <= 0)
                        {
                            itemAdded = true;
                            break;
                        }
                    }
                }
            }
        }

        // If there is still a remaining count to add, try to find an empty slot
        if (!itemAdded && randomCount > 0)
        {
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (!inventory.isfull[i])
                {
                    inventory.isfull[i] = true;
                    GameObject newItem = Instantiate(salvagedItem.itemButtonPrefab, inventory.slots[i].transform, false);
                    DraggableItem draggableItem = newItem.GetComponent<DraggableItem>();
                    Spawn spawnComponent = newItem.GetComponentInChildren<Spawn>();

                    if (draggableItem != null && spawnComponent != null)
                    {
                        // Assign the remaining random count to the new item
                        draggableItem.count = randomCount;
                        draggableItem.RefreshCount();
                        spawnComponent.itemName = salvagedItem.itemNameSalvage;

                        itemAdded = true;
                        break;
                    }
                }
            }
        }

        return itemAdded;
    }

    // Save the current salvage data (number of uses and last salvage time)
    private void SaveSalvageData()
    {
        PlayerPrefs.SetInt("SalvageUses", salvageUses);
        PlayerPrefs.SetString("LastSalvageTime", lastSalvageTime.ToString());
    }

    // Load previously saved salvage data
    private void LoadSalvageData()
    {
        salvageUses = PlayerPrefs.GetInt("SalvageUses", 0);
        string lastTime = PlayerPrefs.GetString("LastSalvageTime", DateTime.MinValue.ToString());
        lastSalvageTime = DateTime.Parse(lastTime);
    }
}
