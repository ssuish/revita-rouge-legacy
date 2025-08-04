using UnityEngine;

public class DropManager : MonoBehaviour
{
    // This method will handle item dropping logic
    public void DropItem(Vector2 dropPosition, GameObject itemPrefab)
    {
        // Instantiate the dropped item prefab at the desired drop position
        Instantiate(itemPrefab, dropPosition, Quaternion.identity);
    }
}