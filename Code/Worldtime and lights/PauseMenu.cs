using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // New Input System namespace


public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu, settingsMenu;
    private PlayerActionMap _inputActions; // Input Action Map for pause functionality
    private bool isPaused = false;

    private void Awake()
    {
        // Initialize the input action map
        _inputActions = new PlayerActionMap();
    }

    private void OnEnable()
    {
        // Enable the input action map
        _inputActions.UI.Enable();

        // Register the callback for the pause action
        _inputActions.UI.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        // Unsubscribe from the pause action and disable the input map
        _inputActions.UI.Pause.performed -= OnPause;
        _inputActions.UI.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }
    public void Settings()
    {
        settingsMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void Quit()
    {
        Time.timeScale = 1; // Ensure time scale is reset before quitting
        GameEventsManagerSO.instance.miscEvents.SaveQuest();
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitMainmenu()
    {
        Time.timeScale = 1; // Ensure time scale is reset before quitting
        SceneManager.LoadScene("MainMenu");
    }

    public void Automata()
    {
        Time.timeScale = 1; // Ensure time scale is reset before quitting
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            SceneManager.LoadScene("Tutorial");
        }
        else
        {
            SceneManager.LoadScene("AutomataBase");
        }
        
        GameEventsManagerSO.instance.miscEvents.SaveQuest();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }
    
    public void ResumeSetting()
    {
        settingsMenu.SetActive(true);
        Time.timeScale = 1;
        isPaused = true;
    }
}