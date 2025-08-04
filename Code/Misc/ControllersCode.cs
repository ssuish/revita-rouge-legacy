using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Import SceneManagement

public class ControllersCode : MonoBehaviour
{
    // Assign the MainMenu scene index in the inspector or set it directly in code
    [SerializeField] private int mainMenuSceneIndex = 0; // Assuming MainMenu is the first scene in the Build Settings

    private void Update()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //Debug.Log("Current Scene Index: " + currentSceneIndex);
        //Debug.Log("Main Menu Scene Index: " + mainMenuSceneIndex);

        // Check if the current active scene index is the MainMenu index
        if (currentSceneIndex == mainMenuSceneIndex)
        {
            //Debug.Log("MainMenu scene detected, destroying ControllersCode object.");
            Destroy(gameObject); // Destroy the object if it's MainMenu
        }
        else
        {
            //Debug.Log("Not in MainMenu, keeping ControllersCode object.");
            DontDestroyOnLoad(gameObject); // Keep the object in other scenes
        }
    }
}