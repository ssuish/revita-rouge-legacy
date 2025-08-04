using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/ItemDatabase", order = 3)]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<Item> itemsResource = new();
    [SerializeField] private List<string> itemDisplayName = new();
    
    // Dictionary to store the display name of the item
    private Dictionary<string, string> itemDisplayNameDict;
    // List of items that cannot be crafted
    private readonly List<string> itemNotCraftable = new()
    {
        "Legumes",
        "Dates",
        "ScrapMetal",
        "Battery",
        "CardBoard",
        "Copper",
        "Plastic",
        "PVC"
    };
    
    private void OnEnable()
    {
        InitializeItemDisplayNameDict();
    }
    
    private void InitializeItemDisplayNameDict()
    {
        itemDisplayNameDict = new Dictionary<string, string>();
        for (var i = 0; i < itemsResource.Count; i++)
        {
            if (i < itemDisplayName.Count)
            {
                itemDisplayNameDict[itemsResource[i].itemName] = itemDisplayName[i];
            }
        }
    }
    
    public Item GetItemResource(string itemName)
    {
        return itemsResource.FirstOrDefault(item => item.itemName == itemName && itemNotCraftable.Contains(itemName));
    }
    
    public List<Item> GetItemsResource()
    {
        return itemsResource.Where(item => itemNotCraftable.Contains(item.itemName)).ToList();
    }
    
    public Item GetItemCraftable(string itemName)
    {
        return itemsResource.FirstOrDefault(item => item.itemName == itemName && !itemNotCraftable.Contains(itemName));
    }
    
    public List<Item> GetItemsCraftable()
    {
        return itemsResource.Where(item => !itemNotCraftable.Contains(item.itemName)).ToList();
    }
    
    public string GetItemDisplayName(string itemName)
    {
        return itemDisplayNameDict.GetValueOrDefault(itemName);
    }
}