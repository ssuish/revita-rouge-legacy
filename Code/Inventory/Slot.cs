using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Slot : MonoBehaviour
{
    private InventoryController inventory;
    public int i;
    public DraggableItem draggableItem;
    public Image slotImage; // The UI Image representing the item
    // Start is called before the first frame update
    void Start()
    {
        draggableItem = GetComponentInChildren<DraggableItem>();
        inventory = FindObjectOfType<InventoryController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update inventory status based on item presence
        if (transform.childCount == 0 && inventory.isfull[i])
        {
            inventory.isfull[i] = false;
        }
    }

    public void DropItem()
    {
        Debug.Log("Name " + draggableItem );
        /*if (amount > 0)
        {
            // Decrease the amount by 1 and update the UI
            amount -= 1;
            UpdateAmountText();

            // Spawn the dropped item
            transform.GetComponentInChildren<Spawn>()?.SpawnDroppedItem();

            // If the draggable item exists, decrement its count as well
            DraggableItem draggableItem = GetComponentInChildren<DraggableItem>();
            if (draggableItem != null)
            {
                draggableItem.count -= 1;
                draggableItem.RefreshCount();  // Update the UI to reflect the new count
            }

            // If the count reaches 0, destroy the item button
            if (amount == 0)
            {
                DestroyItemButton();
            }
        }*/
        if (draggableItem != null && draggableItem.count > 0)
        {
            // Decrease the draggable item count by 1
            draggableItem.count -= 1;
            draggableItem.RefreshCount();

            // Spawn the dropped item in the world
            //transform.GetComponentInChildren<Spawn>()?.SpawnDroppedItem();

            // If the count reaches 0, destroy the item button
            if (draggableItem.count == 0)
            {
                DestroyItemButton();
                inventory.isfull[i] = false;
                
            }
        }
    }

    private void DestroyItemButton()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("ItemButton"))
            {
                Debug.Log("Destroying item: " + child.gameObject.name);
                Destroy(child.gameObject);
                break;
            }
        }
    }
    // Call this method when picking up an item
    public void PickUpItem(DraggableItem newItem)
    {
        if (newItem != null)
        {
            draggableItem = newItem; // Assign the new item
            draggableItem.RefreshCount(); // Update the UI
        }
    }
    /*public void DropItem()
    {
        if (amount > 1)
        {
            amount -= 1;
            transform.GetComponentInChildren<Spawn>().SpawnDroppedItem();
            
        }
        else
        {
            amount -= 1;
            transform.GetComponentInChildren<Spawn>().SpawnDroppedItem();

            // Locate the item button by tag
            foreach (Transform child in transform)
            {
                if (child.CompareTag("ItemButton"))
                {
                    Debug.Log("Destroying item: " + child.gameObject.name);
                    GameObject.Destroy(child.gameObject);
                    break;
                }
            }
        }
    }*/
}
