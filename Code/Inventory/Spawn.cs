using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject itemPrefab;
    private Transform player;
    public string itemName;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        if (itemPrefab == null)
        {
            Debug.LogError("Item prefab is not assigned.");
        }
    }

    // Method to spawn the dropped item near the player's position
    public void SpawnDroppedItem()
    {
        if (itemPrefab == null)
        {
            Debug.LogError("Cannot spawn item: itemPrefab is null.");
            return;
        }

        if (player != null && itemPrefab != null)
        {
            // Spawn the item near the player
            Vector2 spawnPosition = new Vector2(player.position.x, player.position.y + 2);
            Instantiate(itemPrefab, spawnPosition, quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Player reference is missing. Spawning item at current object position.");
            Instantiate(itemPrefab, transform.position, quaternion.identity);
        }
    }

}
