using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneProgressionTrigger : MonoBehaviour
{
    private bool _isTriggered = false;
    //private Leveling LevelingSystem;
    //private InventoryController _inventoryController;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Scene Progression Triggered");
            _isTriggered = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        _isTriggered = false;
    }
    
    public void OnSubmitNextScene()
    {
        if (_isTriggered)
        { 
            //_inventoryController.SaveInventory();
            //LevelingSystem.SaveLevelProgress();
            GameEventsManagerSO.instance.miscEvents.SceneChangeTriggered();
            
            
            
        }
    }

}
