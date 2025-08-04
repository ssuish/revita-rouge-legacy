using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    
    public GameObject[] itemsToSpawn; 
    public int itemCount = 10; 
    public BoxCollider2D[] spawnAreas; 
    public float respawnTimeInMinutes = 10f; 

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
        
        Vector2 randomPosition = GetRandomPosition();

        
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
        
        yield return new WaitForSeconds(respawnTimeInMinutes * 60f);
        

        // Choose a random item from the array
        GameObject itemToSpawn = itemsToSpawn[Random.Range(0, itemsToSpawn.Length)];

        // Instantiate the item at the given position
        GameObject spawnedItem = Instantiate(itemToSpawn, position, Quaternion.identity);

        // Add the spawned item to the list
        spawnedItems.Add(spawnedItem);

        
    }
}

