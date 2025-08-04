using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemSalvage itemSalvage;
    public Image image;
    public TextMeshProUGUI amountText;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public int count = 1;
    public ItemCraft itemCraft;

    private InventoryController inventoryController;
    private ExtendedInventory extendedInventory;
    

    private void Start()
    {
        // Attempt to find InventoryController if it hasn't been set in the Inspector
        if (inventoryController == null)
        {
            inventoryController = FindObjectOfType<InventoryController>();
            extendedInventory = FindObjectOfType<ExtendedInventory>();
        }

        RefreshCount(); // Update the amount text when the item is instantiated
    }

    public void AddToStack(int amountToAdd)
    {
        count += amountToAdd;
        RefreshCount();
    }

    public void RefreshCount()
    {
        amountText.text = count.ToString();
        amountText.gameObject.SetActive(count > 0); // Only show the text if count > 1
    }

    public void SetItemSprite(Sprite newSprite)
    {
        if (newSprite != null && image != null)
        {
            image.sprite = newSprite;  // Update the UI image with the new sprite
            image.enabled = true;      // Make sure the image is enabled
        }
        else
        {
            //Debug.LogWarning("Cannot set item sprite: either the sprite or image is null.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventoryController == null) return;

        // Save the parent before dragging, so you can reparent it later
        parentAfterDrag = transform.parent;

        // Get the original slot index and mark it as empty in isfull
        int originalSlotIndex = parentAfterDrag.GetSiblingIndex();
        if (parentAfterDrag.parent.name == "BackgroundForInventory" && originalSlotIndex >= 0 && originalSlotIndex < inventoryController.isfull.Length)
        {
            
            inventoryController.isfull[originalSlotIndex] = false;  // Mark the slot as empty
            //Debug.Log($"Parent: {parentAfterDrag.parent.name}, Slot Index: {originalSlotIndex}");
            
        }
        else if (parentAfterDrag.parent.name == "ExtendedInventory" && originalSlotIndex >= 0 && originalSlotIndex < extendedInventory.isfull.Length)
        {
            extendedInventory.isfull[originalSlotIndex] = false;  // Mark the slot as empty
            //Debug.Log($"Parent: {parentAfterDrag.parent.name}, Slot Index: {originalSlotIndex}");
        }
        else
        {
            //Debug.LogWarning($"Original slot index {originalSlotIndex} is out of bounds.");
            return;
        }

        // Move the item to the root Canvas, ensuring it remains part of the UI
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null) transform.SetParent(canvas.transform);

        transform.SetAsLastSibling();  // Ensure it’s rendered at the front
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Get the canvas and its RectTransform
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        // Convert the eventData.position (mouse position) to the local position in the canvas RectTransform
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, canvas.worldCamera, out localPoint))
        {
            // Set the local position of the dragged item to the converted local point
            transform.localPosition = localPoint;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;

        // Check if the item was dropped on a valid slot
        if (eventData.pointerEnter != null)
        {
            Transform newParent = eventData.pointerEnter.transform;
            int newSlotIndex = newParent.GetSiblingIndex();
            Transform parentInventory = newParent.parent;

            // Determine which inventory the slot belongs to
            if (parentInventory.name == "BackgroundForInventory" && newSlotIndex >= 0 && newSlotIndex < inventoryController.isfull.Length)
            {
                inventoryController.isfull[newSlotIndex] = true; // Mark main inventory slot as full
                transform.SetParent(newParent); // Set new parent
                transform.localPosition = Vector3.zero; // Align to slot position
                //Debug.Log($"Main Inventory: Slot {newSlotIndex} marked as full.");
            }
            else if (parentInventory.name == "ExtendedInventory" && newSlotIndex >= 0 && newSlotIndex < extendedInventory.isfull.Length)
            {
                extendedInventory.isfull[newSlotIndex] = true; // Mark extended inventory slot as full
                transform.SetParent(newParent); // Set new parent
                transform.localPosition = Vector3.zero; // Align to slot position
                //Debug.Log($"Extended Inventory: Slot {newSlotIndex} marked as full.");
            }
            else
            {
                // If dropped on an invalid slot, return to original position
                ReturnToOriginalPosition();
            }
        }
        else
        {
            // If dropped outside of any slot, return to original position
            ReturnToOriginalPosition();
        }
        
    }
    public void ReturnToOriginalPosition()
    {
        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;

        // Determine which inventory the original slot belongs to
        int originalSlotIndex = parentAfterDrag.GetSiblingIndex();
        Transform parentInventory = parentAfterDrag.parent;

        if (parentInventory.name == "BackgroundForInventory" && originalSlotIndex >= 0 && originalSlotIndex < inventoryController.isfull.Length)
        {
            inventoryController.isfull[originalSlotIndex] = true; // Mark main inventory slot as full
            //Debug.Log($"Returned to Main Inventory: Slot {originalSlotIndex} marked as full.");
        }
        else if (parentInventory.name == "ExtendedInventory" && originalSlotIndex >= 0 && originalSlotIndex < extendedInventory.isfull.Length)
        {
            extendedInventory.isfull[originalSlotIndex] = true; // Mark extended inventory slot as full
            //Debug.Log($"Returned to Extended Inventory: Slot {originalSlotIndex} marked as full.");
        }
    }
    
    
}