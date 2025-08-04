using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class MainMenuHolder : MonoBehaviour
{
    private PlayerActionMap _inputActions;
    public GameObject continueButton; // Button to continue the game
    public GameObject newGameButton; // Button to start a new game
    private const char SPLIT_CHAR = '_';

    private bool hasPlayedBefore = false;
    private bool introCompleted = false;

    private void Awake()
    {
        _inputActions = new PlayerActionMap();
    }

    private void Start()
    {
        // Load game state on start
        LoadState();

        // Set button visibility based on play state
        UpdateButtonVisibility();

    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.UI.StartGame.performed += OnStartGame;
    }

    private void OnDisable()
    {
        _inputActions.UI.StartGame.performed -= OnStartGame;
        _inputActions.Disable();
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private Leveling leveling;
    public void OnStartGame(InputAction.CallbackContext context)
    {
        if (introCompleted)
        {
            LoadSceneByName("AutomataBase");
        }
        else
        {
            hasPlayedBefore = true;
            introCompleted = false;
            SaveState(); // Save the new play state
            leveling.ResetProgressLevel();
            LoadSceneByName("Introduction");
        }
    }

    public void MarkIntroductionComplete()
    {
        hasPlayedBefore = true;
        introCompleted = true;
        SaveState();
        UpdateButtonVisibility(); // Update button visibility after completing the introduction

        //Debug.Log("Completed Introduction");
    }

    private void UpdateButtonVisibility()
    {
        // Show "Continue" button if the player has played before; otherwise, show "New Game" button
        continueButton.SetActive(hasPlayedBefore);
        newGameButton.SetActive(!hasPlayedBefore);
    }
    
    public void SaveState()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "RevitaRogueFinal/Assets/Saves/SavePlay/Playgame.txt");
        
        // Ensure the directory exists
        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using (StreamWriter sw = new StreamWriter(filePath))
        {
            sw.WriteLine("HasPlayedBefore" + SPLIT_CHAR + (hasPlayedBefore ? 1 : 0));
            sw.WriteLine("IntroCompleted" + SPLIT_CHAR + (introCompleted ? 1 : 0));
        }

        //Debug.Log("Main menu state saved.");
    }

    public void LoadState()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "RevitaRogueFinal/Assets/Saves/SavePlay/PlayGame.txt");

        if (!File.Exists(filePath))
        {
            //Debug.LogWarning("Main menu state file not found, loading default state.");
            return;
        }

        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split(SPLIT_CHAR);
                if (parts.Length < 2) continue;

                if (parts[0] == "HasPlayedBefore")
                {
                    hasPlayedBefore = parts[1] == "1";
                }
                else if (parts[0] == "IntroCompleted")
                {
                    introCompleted = parts[1] == "1";
                }
            }
        }

        Debug.Log("Main menu state loaded.");
    }
    public void ResetProgress()
    {
        hasPlayedBefore = false;
        introCompleted = false;

        // Delete the save file
        string filePath = Path.Combine(Application.persistentDataPath, "RevitaRogueFinal/Assets/Saves/SavePlay/Playgame.txt");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // Update button visibility to show "New Game" only
        UpdateButtonVisibility();
        //Debug.Log("Game progress reset.");
    }
}
