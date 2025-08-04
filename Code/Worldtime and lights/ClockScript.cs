using System;
using UnityEngine;
using TMPro;

public class ClockScript : MonoBehaviour
{
    public TMP_Text currentTimeText;
    private WorldTime worldTime; // Format: hh:mm
    //private float playStartTime;

    private void Start()
    {
        // Ensure the currentTimeText is assigned
        if (currentTimeText == null)
        {
            //Debug.LogError("currentTimeText is not assigned in the Inspector.");
            return;
        }

        // Get the WorldTime component from the scene
        worldTime = FindObjectOfType<WorldTime>();

        if (worldTime != null)
        {
            // Subscribe to the WorldTimeChanged event
            worldTime.WorldTimeChanged += OnWorldTimeChanged;
        }
    }

    private void OnWorldTimeChanged(object sender, TimeSpan e)
    {
        if (currentTimeText != null)
        {
            if (worldTime.isOfflineMode)
            {
                // Show the in-game offline time if in offline mode
                currentTimeText.text = "Offline Time:\n " + DateTime.Now.ToString(@"hh\:mm");
            }
            else
            {
                // Show the real-world time if online mode
                currentTimeText.text = "Online Time:\n " + e.ToString(@"hh\:mm");
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid potential memory leaks
        if (worldTime != null)
        {
            worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }
    }
}


