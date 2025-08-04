using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class Salvage2 : MonoBehaviour
{
    public static Salvage2 Instance { get; private set; }
    public SalvageSlot salvageSlot;
    public Button salvageButton;
    public List<ItemSalvage> salvageableItems; // List of items that can be salvaged
  // List of items that can be returned after salvaging
    private InventoryController inventory;
    public Leveling LevelingSystem;

    private int salvageUses = 0; // Count of salvage uses
    private const int maxSalvageUses = 20; // Maximum number of uses allowed
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
        if (inventory == null)
        {
            //Debug.LogError("InventoryController not found in the scene.");
        }

        // Load saved data for salvage uses and last salvage time
        LoadSalvageData();
        
        cooldowntext.gameObject.SetActive(false);
    }
    
    private void Update()
    {
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
        // Check if the player can salvage based on uses and cooldown
        if (CanSalvage())
        {

            if (salvageSlot != null && salvageSlot.itemSalvage != null)
            {
                bool returnToOriginalSlot;
                // Check if the item in the slot is in the salvageableItems list
                if (IsItemSalvageable(salvageSlot.itemSalvage, out returnToOriginalSlot))
                {
                    // Pick a random item from the SalvageResults list
                    ItemSalvage randomItem = salvageableItems[Random.Range(0, salvageableItems.Count)];
                    //Debug.Log("Salvaging item and adding random result: " + randomItem.itemNameSalvage);
                    // Add experience points
                    if (LevelingSystem.Level < 20)
                    {
                        int randomExp = Random.Range(50, 80);
                        //Debug.Log("Adding EXP: " + randomExp);
                        LevelingSystem.CurrentXp += randomExp;
                        LevelingSystem.ExperienceController();
                    }

                    // Clear the salvage slot after successfully salvaging the item
                    salvageSlot.ClearSlot();

                    // Increment salvage uses and update the last salvage time
                    salvageUses++;
                    lastSalvageTime = DateTime.Now;
                    SaveSalvageData();
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
            else
            {
                //Debug.LogWarning("Salvage cooldown in effect. Try again later.");
                return false;
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
