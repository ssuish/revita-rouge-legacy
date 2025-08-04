using UnityEngine;

[System.Serializable]
public class Item
{
    public int itemID;
    public string itemName;
    public Sprite itemSprite;
    public GameObject prefab; // Reference to the prefab for this item

    public object GetComponent<T>()
    {
        throw new System.NotImplementedException();
    }
}