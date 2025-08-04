using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SalvageSlot : MonoBehaviour, IDropHandler
{
    public ItemSalvage itemSalvage; 
    public int index; 

    
    public Image slotImage; 
    public TextMeshProUGUI countText;

    public Sprite defaultSprite; 
    private DraggableItem currentDraggableItem; 

    //Enum to differentiate between Crafting and Salvaging modes
    public enum SlotMode { Crafting, Salvaging }
    public SlotMode currentMode;

    // Reference to the Salvage manager
    public Salvage salvageManager;

    private void Start()
    {
        // Find the Salvage manager in the scene
        salvageManager = FindObjectOfType<Salvage>();

        if (salvageManager == null)
        {
            //Debug.LogError("SalvageManager not found in the scene.");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            // Ensure the dragged item has an itemSalvage reference
            if (draggableItem.itemSalvage != null)
            {
                //Debug.Log($"Dropped salvage item: {draggableItem.itemSalvage.itemNameSalvage} with count {draggableItem.count}");

                // Set the slot image to the dragged item's image
                slotImage.sprite = draggableItem.image.sprite;
                slotImage.color = Color.white; // Ensure the image is visible

                // Update the count text
                countText.text = draggableItem.count.ToString();
                countText.gameObject.SetActive(draggableItem.count > 1); // Show only if count > 1

                // Assign the itemSalvage to this slot
                itemSalvage = draggableItem.itemSalvage;

                currentDraggableItem = draggableItem;
                draggableItem.gameObject.SetActive(false); 

                if (currentMode == SlotMode.Salvaging)
                {
                    //.Log($"Salvaging: Added {itemSalvage?.itemNameSalvage} to salvage slot {index}.");
                    salvageManager.SalvageItem();
                    //ClearSlot();
                }
            }
            else
            {
                //Debug.LogError("Dragged item does not have an itemSalvage reference!");
            }
        }
        else
        {
            //Debug.LogError("No draggable item found on drop.");
        }
    }
    
    public void ReturnToOriginalSlot()
    {
        if (currentDraggableItem != null)
        {
            currentDraggableItem.gameObject.SetActive(true);
            //Debug.LogWarning("Item returned to the original slot.");
        }
        ClearSlot();
    }


    // Method to clear the slot
    public void ClearSlot()
    {
        itemSalvage = null;
        slotImage.sprite = defaultSprite; // Reset to default sprite
        slotImage.color = new Color(0, 0, 0, 0); // Make image invisible

        countText.text = "";
        countText.gameObject.SetActive(false); // Hide the count text
    }
}
