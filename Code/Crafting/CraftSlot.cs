using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CraftSlot : MonoBehaviour, IDropHandler
{
    public ItemCraft itemCraft;
    public int index; 

    
    public Image slotImage; 
    public TextMeshProUGUI countText; 

    public Sprite defaultSprite;
    public DraggableItem currentDraggableItem;
    // Enum to differentiate between Crafting and Salvaging modes
    public enum SlotMode { Crafting, Salvaging }
    public SlotMode currentMode;

    // Reference to both managers
    public CraftingManager craftingManager;
    public Salvage salvageManager;
    
    private void Start()
    {
        // Assuming you have a way to assign this
        salvageManager = FindObjectOfType<Salvage>();

        if (salvageManager == null)
        {
            //Debug.LogError("SalvageManager not found in the scene.");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Get the item being dragged
        DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        //Destroy(gameObject);

        if (draggableItem != null)
        {
            //Debug.Log($"Dropped item: {draggableItem.itemCraft?.itemName ?? "None"} with count {draggableItem.count}");

            if (itemCraft != null)
            {
                return;
            }
            // Set the slot image to the dragged item's image
            slotImage.sprite = draggableItem.image.sprite;
            slotImage.color = Color.white; // Make sure the image is visible

            // Set the count text to the dragged item's count
            countText.text = draggableItem.count.ToString();
            countText.gameObject.SetActive(draggableItem.count > 1); // Show count text only if count > 1

            // Assign the itemCraft based on the dragged item
            itemCraft = draggableItem.itemCraft; // Correct reference to the actual itemCraft

            // Keep a reference to the draggable item
            currentDraggableItem = draggableItem;

            
            draggableItem.gameObject.SetActive(false);
            // Handle different behavior based on the current mode (Crafting or Salvaging)
            if (currentMode == SlotMode.Crafting)
            {
                //Debug.Log($"Crafting: Added {itemCraft.itemName} to craft slot {index}.");
            }
            else if (currentMode == SlotMode.Salvaging)
            {
                //Debug.Log($"Salvaging: Added {itemCraft.itemName} to salvage slot {index}.");
                // Trigger the salvage process through the SalvageManager
                salvageManager.SalvageItem();
                ClearSlot(); // Automatically clear the slot after salvaging
            }

            //Debug.Log($"CraftSlot {index}: Assigned itemCraft = {itemCraft?.itemName ?? "None"} with count = {draggableItem.count}");
        }
    }
    
    public void RestoreDraggableItem()
    {
        if (currentDraggableItem != null)
        {
            currentDraggableItem.gameObject.SetActive(true); // Reactivate the draggable item
            currentDraggableItem.transform.SetParent(currentDraggableItem.parentAfterDrag); // Move back to the original parent
            currentDraggableItem.transform.localPosition = Vector3.zero; // Reset position
        }
    }


  
    // Method to clear the slot
    public void ClearSlot()
    {
        // Reset the slot image and text
        itemCraft = null;
        slotImage.sprite = defaultSprite;
        slotImage.color = new Color(0, 0, 0, 0); // Make image invisible

        countText.text = "0";
        //countText.gameObject.SetActive(false);

        if (currentDraggableItem != null)
        {
            if (currentDraggableItem.count <= 0)
            {
                //Debug.Log($"CraftSlot {index} used up. Local position: {currentDraggableItem.transform.localPosition}");
                currentDraggableItem.transform.localPosition = Vector3.zero;
                Destroy(currentDraggableItem.gameObject); // If the item is fully used, destroy it
                currentDraggableItem = null;
                //slotImage.sprite = null;
            }
            else
            {
                currentDraggableItem.transform.SetParent(null);
                //Debug.Log($"CraftSlot {index} used up. Local position: {currentDraggableItem.transform.localPosition}");
                currentDraggableItem.transform.localPosition = Vector3.zero;
                Destroy(currentDraggableItem.gameObject);
                //currentDraggableItem.gameObject.SetActive(true); // Reactivate if some amount is left
                currentDraggableItem = null;
            }
        }
        
        
        gameObject.SetActive(false);  
        gameObject.SetActive(true);   

    }
    
}
