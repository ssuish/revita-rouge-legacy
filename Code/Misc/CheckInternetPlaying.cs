using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Net.NetworkInformation;
using Ping = System.Net.NetworkInformation.Ping; // Import for ping functionality


public class CheckInternetPlaying : MonoBehaviour
{
    //Singleton Instance 
    public static CheckInternetPlaying Instance { get; private set; }

    [SerializeField] private GameObject internetChecking;

    [FormerlySerializedAs("ConnectedTxt")] [SerializeField]
    private TextMeshProUGUI connectedTxt;

    [FormerlySerializedAs("ConErrorText")] [SerializeField]
    private TextMeshProUGUI conErrorText;

    [SerializeField] private Button tryAgainBut;
    [SerializeField] private Button playbutton;
    [SerializeField] private Button playOfflineButton;

    // Controls
    [FormerlySerializedAs("Actions")] [SerializeField]
    private GameObject actions;

    [FormerlySerializedAs("Pause")] [SerializeField]
    private GameObject pause;

    [FormerlySerializedAs("Features")] [SerializeField]
    private GameObject features;

    [FormerlySerializedAs("Stats")] [SerializeField]
    private GameObject stats;

    // Reference to the WorldTime script
    //[SerializeField] private WorldTime worldTime;

    private bool _isPaused = false;
    private bool _isChecking = false;
    private float _checkInterval = 7f; // Check every 5 seconds
    private int _failedAttempts = 0; // Track the number of failed connection attempts
    private const int MaxFailedAttempts = 3; // Number of failed attempts before showing play offline
    private bool _wasOnline = false; // Track if the player was online initially

    void Awake()
    {
        // Singleton implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy any duplicate instances
        }
        else
        {
            Instance = this;
            
        }
    }

    void Start()
    {
        internetChecking.SetActive(false); // Hide the UI initially
        playOfflineButton.gameObject.SetActive(false);
        //worldTime = FindObjectOfType<WorldTime>();
        StartCoroutine(ContinuousCheckInternetConnection());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator ContinuousCheckInternetConnection()
    {
        while (true) // Infinite loop to keep checking connection
        {
            yield return CheckInternetConnection();
            yield return new WaitForSeconds(_checkInterval); // Wait for the next check
        }
    }


    private IEnumerator CheckInternetConnection()
    {
        if (_isChecking) yield break;
        _isChecking = true;

        bool isConnected = false;

        // Attempt a ping to Google's DNS
        Ping ping = new Ping();
        PingReply reply = ping.Send("8.8.8.8", 60000); // Ping with a 2-second timeout

        if (reply != null && reply.Status == IPStatus.Success)
        {
            isConnected = true;
        }
        else
        {
            Debug.LogError("Ping to Google DNS failed.");
        }

        HandleConnectionResult(isConnected);

        _isChecking = false;
    }
    
    // Custom certificate handler to bypass SSL verification
    public class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // Always return true to bypass SSL verification
        }
    }

    private void HandleConnectionResult(bool isConnected)
    {
        if (!isConnected) // If all URLs failed
        {
            _failedAttempts++;

            if (_wasOnline)
            {
                // If the player was online and now offline, restart the game in offline mode
                PlayOffline();
            }
            else
            {
                // If the player is already offline from the start, continue the game without showing UI
                ResumeGame();
            }
        }
        else
        {
            // Connection successful, reset failed attempts counter
            _failedAttempts = 0;
            _wasOnline = true; // Mark the player as having been online

            // Hide connection error UI
            internetChecking.SetActive(false);
            conErrorText.gameObject.SetActive(false);
            connectedTxt.gameObject.SetActive(false);
            tryAgainBut.gameObject.SetActive(false);
            playbutton.gameObject.SetActive(true);
            playOfflineButton.gameObject.SetActive(false); // Hide play offline button

            // OTHER Buttons
            actions.SetActive(true);
            pause.SetActive(true);
            features.SetActive(true);
            stats.SetActive(true);
            ResumeGame();
        }
    }

    public void Play()
    {
        // Unpause the game when Play is pressed
        internetChecking.SetActive(false);
        ResumeGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // Pause the game by setting timescale to 0
        _isPaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the game by setting timescale back to 1
        _isPaused = false;
    }

    public void PlayOffline()
    {
        // Log that we are quitting to go to offline mode
        Debug.Log("Quitting and loading offline mode...");
        internetChecking.gameObject.SetActive(true);
        conErrorText.gameObject.SetActive(true);
        playbutton.gameObject.SetActive(false);
        playOfflineButton.gameObject.SetActive(false);

        StartCoroutine(QuitAfterDelay(5f));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator QuitAfterDelay(float delay)
    {
        // Wait for the specified delay (in seconds)
        yield return new WaitForSeconds(delay);
        
        SaveGame();
        
        // Quit the game to restart it in offline mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private void SaveGame()
    {
        // Save the game state
        InventoryController.Instance.SaveInventory();
    }
}