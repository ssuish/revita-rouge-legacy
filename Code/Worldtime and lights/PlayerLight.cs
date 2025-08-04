using UnityEngine;
using System;

public class Playerlight : MonoBehaviour
{
    public GameObject playerLight;  // Reference to the Light component
    private bool isPlayerLightOn = false;

    // TimeSpan for when the light should turn on and off
    public TimeSpan glowStartTime = new TimeSpan(18, 0, 0);  // 6 PM
    public TimeSpan glowEndTime = new TimeSpan(5, 0, 0);     // 5 AM

    private void OnEnable()
    {
        // Subscribe to the WorldTime event
        FindObjectOfType<WorldTime>().WorldTimeChanged += OnWorldTimeChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from the WorldTime event if the WorldTime object exists
        var worldTime = FindObjectOfType<WorldTime>();
        if (worldTime != null)
        {
            worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }
    }


    private void Start()
    {
        // Ensure the streetlight is off initially
        if (playerLight != null)
        {
            playerLight.SetActive(false);
        }
    }

    private void OnWorldTimeChanged(object sender, TimeSpan currentTime)
    {
        // Check if the current in-game time is within the glow period
        if (IsWithinGlowPeriod(currentTime))
        {
            if (!isPlayerLightOn)
            {
                ToggleStreetlight(true);  // Turn on the streetlight
            }
        }
        else
        {
            if (isPlayerLightOn)
            {
                ToggleStreetlight(false); // Turn off the streetlight
            }
        }
    }

    private bool IsWithinGlowPeriod(TimeSpan currentTime)
    {
        // If the end time is before the start time, the glow period spans midnight
        if (glowEndTime < glowStartTime)
        {
            return (currentTime >= glowStartTime) || (currentTime <= glowEndTime);
        }
        else
        {
            return (currentTime >= glowStartTime) && (currentTime <= glowEndTime);
        }
    }

    private void ToggleStreetlight(bool turnOn)
    {
        if (playerLight != null)
        {
            isPlayerLightOn = turnOn;
            playerLight.SetActive(turnOn);
        }
    }
}
