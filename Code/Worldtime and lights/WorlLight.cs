using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class WorlLight : MonoBehaviour
{
    private Light2D _light;
    [SerializeField] private WorldTime _worldTime;
    [SerializeField] private Gradient _gradient;

    private bool isCurrentlyOfflineMode;
    
    private void Awake()
    {
        _light = GetComponent<Light2D>();
        if (_worldTime == null)
        {
            Debug.LogError("WorldTime reference is not set in the inspector.");
            return;
        }

        isCurrentlyOfflineMode = _worldTime.isOfflineMode;
        SubscribeToWorldTimeChanged();
    }

    private void OnDestroy()
    {
        UnsubscribeFromWorldTimeChanged();
    }

    // This method gets triggered when the time changes, based on the server time from WorldTime
    private void OnWorldTimeChangedOnline(object sender, TimeSpan newTime)
    {
        UpdateLight((float)newTime.TotalSeconds / _worldTime.dayLengthInSeconds);
    }

    private void OnWorldTimeChangedOffline(object sender, TimeSpan newTime)
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan elapsedSinceMidnight = currentTime - currentTime.Date;
        UpdateLight((float)elapsedSinceMidnight.TotalSeconds / _worldTime.dayLengthInSeconds);
    }
    
    private void UpdateLight(float percentOfDay)
    {
        Color lightColor = _gradient.Evaluate(percentOfDay);
        _light.color = lightColor;

        // Adjust light intensity if needed
        _light.intensity = Mathf.Lerp(1.0f, 1.0f, percentOfDay);
    }
    
    private void Update()
    {
        if (_worldTime.isOfflineMode != isCurrentlyOfflineMode)
        {
            UnsubscribeFromWorldTimeChanged();
            isCurrentlyOfflineMode = _worldTime.isOfflineMode;
            SubscribeToWorldTimeChanged();
        }
    }
    
    private void SubscribeToWorldTimeChanged()
    {
        if (isCurrentlyOfflineMode)
        {
            _worldTime.WorldTimeChanged += OnWorldTimeChangedOffline;
        }
        else
        {
            _worldTime.WorldTimeChanged += OnWorldTimeChangedOnline;
        }
    }

    private void UnsubscribeFromWorldTimeChanged()
    {
        _worldTime.WorldTimeChanged -= OnWorldTimeChangedOnline;
        _worldTime.WorldTimeChanged -= OnWorldTimeChangedOffline;
    }
}

