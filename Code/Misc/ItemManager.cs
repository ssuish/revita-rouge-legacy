using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public int itemCount { get; private set; }

    private void Awake()
    {
        itemCount = 0;
    }

    private void OnEnable()
    {
        GameEventsManagerSO.instance.itemEvents.onItemCollectedWithCount += ItemCollectedWithCount;
    }

    private void OnDisable()
    {
        GameEventsManagerSO.instance.itemEvents.onItemCollectedWithCount -= ItemCollectedWithCount;
    }

    private void ItemCollectedWithCount(string itemName, int count)
    {
        //Debug.Log($"Event received: Item = {itemName}, Count = {count}");
        itemCount = count;
        //Debug.Log($"Updated itemCount: {itemCount}");
    }
    
}
