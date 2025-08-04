using System;
using System.Collections.Generic;

public class ItemEvents
{
    public event Action<string, int> onItemCollectedWithCount;

    public void ItemCollectedWithCount(string item, int count)
    {
        if (onItemCollectedWithCount != null)
        {
            onItemCollectedWithCount(item, count);
        }
    }
    
    public event Action<Dictionary<string, int>> onItemInventoryCount;
    public void ItemInventoryCount(Dictionary<string, int> item)
    {
        if (onItemInventoryCount != null)
        {
            onItemInventoryCount(item);
        }
    }
    
    public event Action onItemUsed;
    public void ItemUsed()
    {
        if (onItemUsed != null)
        {
            onItemUsed();
        }
    }

    public event Action<string, bool>  onItemCrafted;
    public void ItemCrafted(string item, bool success)
    {
        if (onItemCrafted != null)
        {
            onItemCrafted(item, success);
        }
    }
}
