using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DatesSpawner : MonoBehaviour
{
    // Singleton instance
    //public static DatesSpawner Instance { get; private set; }
    public GameObject[] itemsToSpawn; // Array of item prefabs to spawn
    public int itemCount = 10; // Number of items to spawn
    public BoxCollider2D[] spawnAreas; // Array of areas within which to spawn items
    public float respawnTimeInMinutes = 10f; // Time in minutes before an item respawns

    private List<GameObject> spawnedItems = new List<GameObject>();

    
    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        foreach (var spawnArea in spawnAreas)
        {
            Vector2 position = GetRandomPositionInArea(spawnArea);
            GameObject itemToSpawn = itemsToSpawn[Random.Range(0, itemsToSpawn.Length)];
            GameObject spawnedItem = Instantiate(itemToSpawn, position, Quaternion.identity);
            spawnedItems.Add(spawnedItem);
        }

        // Then, spawn the remaining items in random spawn areas
        int remainingItems = itemCount - spawnAreas.Length;
        for (int i = 0; i < remainingItems; i++)
        {
            SpawnItem();
        }
    }

    Vector2 GetRandomPositionInArea(BoxCollider2D spawnArea)
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    void SpawnItem()
    {
        // Get a random position within one of the spawn areas
        Vector2 randomPosition = GetRandomPosition();

        // Choose a random item from the array
        GameObject itemToSpawn = itemsToSpawn[Random.Range(0, itemsToSpawn.Length)];

        // Instantiate the item at the random position
        GameObject spawnedItem = Instantiate(itemToSpawn, randomPosition, Quaternion.identity);
        
        // Add the spawned item to the list
        spawnedItems.Add(spawnedItem);
        
    }

    Vector2 GetRandomPosition()
    {
        // Choose a random spawn area
        BoxCollider2D randomSpawnArea = spawnAreas[Random.Range(0, spawnAreas.Length)];

        // Get the bounds of the chosen spawn area
        Bounds bounds = randomSpawnArea.bounds;

        // Get a random position within the bounds
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(x, y);
    }

    public void StartRespawnCoroutine(Vector2 position)
    {
        StartCoroutine(RespawnItem(position));
    }

    IEnumerator RespawnItem(Vector2 position)
    {
        // Convert minutes to seconds for the respawn time
        yield return new WaitForSeconds(respawnTimeInMinutes * 60f);
   

        // Choose a random item from the array
        GameObject itemToSpawn = itemsToSpawn[Random.Range(0, itemsToSpawn.Length)];

        // Instantiate the item at the given position
        GameObject spawnedItem = Instantiate(itemToSpawn, position, Quaternion.identity);

        // Add the spawned item to the list
        spawnedItems.Add(spawnedItem);

    }
}

