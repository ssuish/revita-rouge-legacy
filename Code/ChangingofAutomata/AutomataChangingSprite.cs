using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class AutomataChangingSprite : MonoBehaviour
{
    public Sprite[] sprites; // Array of sprites for different levels
    public Sprite[] upgradingSprites; // Array of sprites for the upgrade process
    public GameObject[] upgradeObjects; // Array of GameObjects to show when upgraded
    public float upgradeTime = 5.0f; // Time it takes to upgrade
    public Button upgradeButton; // The button to trigger the upgrade
    public TextMeshPro countdownText; // Text UI element for showing the countdown

    private int currentLevel = 0; // Current level of the sprite
    private bool isUpgrading = false; // Whether the sprite is currently upgrading
    private SpriteRenderer spriteRenderer;
    
    private const char SPLIT_CHAR = '_';


    // List of required items for each upgrade
    private string[] requiredItems = new string[] { "SolarPanel", "RainwaterHarvester", "FuelCellElectricEngine" };

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize with the first sprite
        if (sprites.Length > 0)
        {
            spriteRenderer.sprite = sprites[0];
        }

        // Set all upgrade objects to inactive initially, except the first one
        for (int i = 0; i < upgradeObjects.Length; i++)
        {
            upgradeObjects[i].SetActive(i == 0); // Only the first one should be active initially
        }

        // Add button listener
        upgradeButton.onClick.AddListener(Upgrade);

        // Hide countdown text at the start
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        LoadState();
    }
    public void SaveState()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "RevitaRogueFinal/Assets/Saves/SaveAutomata/AutoMataState.txt");

        // Ensure the directory exists
        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using (StreamWriter sw = new StreamWriter(filePath))
        {
            // Save the current level
            sw.WriteLine("CurrentLevel" + SPLIT_CHAR + currentLevel);

            // Save whether it's upgrading
            sw.WriteLine("IsUpgrading" + SPLIT_CHAR + (isUpgrading ? 1 : 0));
            
            sw.WriteLine("UpgradeObjectsLength" + SPLIT_CHAR + upgradeObjects.Length);

        }

    }
    
    public void LoadState()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "RevitaRogueFinal/Assets/Saves/SaveAutomata/AutoMataState.txt");

        if (!File.Exists(filePath))
        {
            return;
        }

        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split(SPLIT_CHAR);
                if (parts.Length < 2)
                {
                    continue;
                }

                string key = parts[0];
                if (!int.TryParse(parts[1], out int value))
                {
                    continue;
                }

                if (key == "CurrentLevel")
                {
                    SetCurrentLevel(value);
                }
                else if (key == "IsUpgrading")
                {
                    SetIsUpgrading(value == 1);
                }
                else if (key == "UpgradeObjectsLength")
                {
                    // Ensure the saved length matches the current length, or log a warning if they differ
                    if (value != upgradeObjects.Length)
                    {
                    }
                }
            }
        }
        UpdateUpgradeObjects();
    }
    private void UpdateUpgradeObjects()
    {
        // Deactivate all upgrade objects and activate the one that matches the current level
        for (int i = 0; i < upgradeObjects.Length; i++)
        {
            upgradeObjects[i].SetActive(i == currentLevel);
        }
    }
    private void SetCurrentLevel(int level)
    {
        currentLevel = level;
        if (level < sprites.Length)
        {
            spriteRenderer.sprite = sprites[currentLevel];
        }
    }

    private void SetIsUpgrading(bool upgrading)
    {
        isUpgrading = upgrading;
    }

    public void Upgrade()
    {
        if (!isUpgrading && currentLevel < sprites.Length - 1)
        {
            string requiredItem = requiredItems[currentLevel]; // Get the required item for the current level

            // Check if the required item is in the inventory
            if (HasItemInInventory(requiredItem))
            {
                RemoveItemFromInventory(requiredItem); // Remove the item before starting the upgrade
                StartCoroutine(UpgradeCoroutine()); // Start the upgrade coroutine
            }
            
        }
    }

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

    private IEnumerator UpgradeCoroutine()
    {
        isUpgrading = true;

        // Set the sprite to the corresponding upgrading sprite based on the current level
        if (currentLevel < upgradingSprites.Length)
        {
            spriteRenderer.sprite = upgradingSprites[currentLevel];
        }

        // Activate countdown text
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        // Countdown loop
        float remainingTime = upgradeTime;
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            
            // Update the countdown text
            if (countdownText != null)
            {
                countdownText.text = "Upgrading in " + Mathf.Ceil(remainingTime).ToString() + "s";
            }

            yield return null; // Wait for the next frame
        }

        // Deactivate the current upgrade object
        if (currentLevel < upgradeObjects.Length - 1)
        {
            // Deactivate the first active object
            upgradeObjects[currentLevel].SetActive(false);

            // Ensure the next one is still active and activate the next one after that
            upgradeObjects[currentLevel + 1].SetActive(true);
            if (currentLevel + 1 < upgradeObjects.Length)
            {
                upgradeObjects[currentLevel + 1].SetActive(true);
            }
        }

        // Upgrade to the next sprite (level)
        currentLevel++;
        spriteRenderer.sprite = sprites[currentLevel];

        // Activate the next upgrade object if available
        if (currentLevel < upgradeObjects.Length)
        {
            upgradeObjects[currentLevel].SetActive(true);
        }

        // Call the event to trigger the upgrade subscriptions (Dialogue, Quest, etc.)
        GameEventsManagerSO.instance.miscEvents.AutomataUpgraded();

        // Hide countdown text after upgrade
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        isUpgrading = false;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public bool IsUpgrading()
    {
        return isUpgrading;
    }
    
    public void ResetState()
    {
        // Reset level to 0 and upgrading state to false
        currentLevel = 0;
        isUpgrading = false;

        // Set the initial sprite
        if (sprites.Length > 0)
        {
            spriteRenderer.sprite = sprites[0];
        }

        // Deactivate all upgrade objects except the first one
        for (int i = 0; i < upgradeObjects.Length; i++)
        {
            upgradeObjects[i].SetActive(i == 0); // Only the first one should be active initially
        }

        // Hide countdown text
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
            countdownText.text = ""; // Clear any text in the countdown
        }

        // Save the reset state if necessary
        SaveState();

    }

    private void OnApplicationQuit()
    {
        SaveState();
    }
}
