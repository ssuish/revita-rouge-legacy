using UnityEngine;

public class ItemSalvage : MonoBehaviour
{
    public string itemNameSalvage; // The name of the item that can be salvaged
    public GameObject itemButtonPrefab; // The prefab for displaying the item in the inventory
    public int count; // The quantity of the item

    public ItemSalvage(string itemName, GameObject prefab, int itemCount)
    {
        itemNameSalvage = itemName;
        itemButtonPrefab = prefab;
        count = itemCount; // Assigning the count value
    }
}