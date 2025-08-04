using UnityEngine;
using System;

public class Streetlight : MonoBehaviour
{
    public GameObject streetlight;
    private bool isStreetlightOn = false;

    public TimeSpan glowStartTime = new TimeSpan(18, 0, 0);  // 6 PM
    public TimeSpan glowEndTime = new TimeSpan(6, 0, 0);     // 6 AM

    private void OnEnable()
    {
        FindObjectOfType<WorldTime>().WorldTimeChanged += OnWorldTimeChanged;
    }

    private void OnDisable()
    {
        var worldTime = FindObjectOfType<WorldTime>();
        if (worldTime != null)
        {
            worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }
    }

    private void Start()
    {
        if (streetlight != null)
        {
            var worldTime = FindObjectOfType<WorldTime>();
            if (worldTime != null)
            {
                TimeSpan currentTime = worldTime.GetCurrentTime();
                ToggleStreetlight(IsWithinGlowPeriod(currentTime));
            }
            else
            {
                streetlight.SetActive(false);
            }
        }
    }

    private void OnWorldTimeChanged(object sender, TimeSpan currentTime)
    {
        if (IsWithinGlowPeriod(currentTime))
        {
            if (!isStreetlightOn)
            {
                ToggleStreetlight(true);
            }
        }
        else
        {
            if (isStreetlightOn)
            {
                ToggleStreetlight(false);
            }
        }
    }

    private bool IsWithinGlowPeriod(TimeSpan currentTime)
    {
        if (glowEndTime < glowStartTime)
        {
            return (currentTime >= glowStartTime) || (currentTime < glowEndTime);
        }
        else
        {
            return (currentTime >= glowStartTime) && (currentTime < glowEndTime);
        }
    }

    private void ToggleStreetlight(bool turnOn)
    {
        if (streetlight != null)
        {
            isStreetlightOn = turnOn;
            streetlight.SetActive(turnOn);
        }
    }
}