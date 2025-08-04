using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class QuestTimer : MonoBehaviour
{
    [SerializeField] private WorldTime _worldTime;
    private bool _isNightMode;
    private int _scheduledTimeRangeX;
    private int _scheduledTimeRangeY;
    private TimeSpan _elapsedTime;
    private TimeSpan _gameTime;
    private readonly Dictionary<string, int> _timers = new();
    private DateTime _startTime;
    private DateTime _endTime;
    private bool _isRunning;

    private void Start()
    {
        _scheduledTimeRangeX = 18;
        _scheduledTimeRangeY = 6;
        _elapsedTime = DateTime.Now.TimeOfDay;
        _startTime = DateTime.Now;
        _isRunning = true;
    }
    
    private void OnEnable()
    {
        GameEventsManagerSO.instance.miscEvents.onSetTimer += SetTimer;
        _worldTime.WorldTimeChanged += OnWorldTimeChanged;
        LoadElapsedTime();
    }

    private void OnDisable()
    {
        GameEventsManagerSO.instance.miscEvents.onSetTimer -= SetTimer;
        _worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        SaveElapsedTime();
    }
    
    private void SetTimer(string timerName, int time)
    {
        _timers[timerName] = time;
    }
    
    public void StopTimer()
    {
        _isRunning = false;
        SaveElapsedTime();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnWorldTimeChanged(object sender, TimeSpan currentTime)
    {
        TimeSpan lastUpdateTime = _worldTime.GetLastUpdateTime().TimeOfDay;
        TimeSpan newElapsedTime;

        if (_worldTime.isOfflineMode)
        {
            // Get the in-game offline time if in offline mode
            newElapsedTime = DateTime.Now.TimeOfDay - lastUpdateTime;
        }
        else
        {
            // Get the real-world time if online mode
            newElapsedTime = currentTime - lastUpdateTime;
        }
        
        GameEventsManagerSO.instance.miscEvents.InternetStatusChange(!_worldTime.isOfflineMode);

        // Add the new elapsed time to the total game time
        _gameTime += newElapsedTime;
        //Debug.Log("Game Time: " + _gameTime);
        
        // Reset elapsedTime to zero and accumulate the new elapsed time
        _elapsedTime = currentTime;
        //Debug.Log("Elapsed Time: " + _elapsedTime);

        // Update the last update time
        _worldTime.SetLastUpdateTime(DateTime.Now);

        // Check if the elapsed time is within the night mode range (6 PM - 6 AM)
        CheckIfNightMode();
        
        // Make a countdown timer for each timer in the dictionary
        RunTimers();
        
        // Check if days have passed
        TriggerOnDayPassed();
    }

    private void TriggerOnDayPassed()
    {
        // if (elapsedTime.Hours == 0 && elapsedTime.Minutes == 0)
        // {
        //     GameEventsManagerSO.instance.miscEvents.DayPassed();
        // }
        
        // Check if current day is more recent than saved day
        if (_elapsedTime.Days > _gameTime.Days)
        {
            GameEventsManagerSO.instance.miscEvents.DayPassed();
        }
    }

    private void RunTimers()
    {
        foreach (var key in _timers.Keys.ToList())
        {
            if (_timers[key] == 0)
            {
                // Trigger the game event with the timer name
                GameEventsManagerSO.instance.miscEvents.TimerEnded(key);
            }
            else
            {
                _timers[key]--;
            }
        }
    }

    private void CheckIfNightMode()
    {
        _isNightMode = _elapsedTime.Hours is >= 18 or < 6;
        
        // Trigger the game event with the night mode status
        GameEventsManagerSO.instance.miscEvents.NightTime(_isNightMode);
    }
    
    private void SaveElapsedTime()
    {
        if (_isRunning)
        {
            _gameTime += DateTime.Now - _startTime;
            _startTime = DateTime.Now;
        }
        PlayerPrefs.SetString("TotalElapsedTime", _gameTime.ToString());
        PlayerPrefs.Save();
    }

    private void LoadElapsedTime()
    {
        _gameTime = PlayerPrefs.HasKey("TotalElapsedTime") ? TimeSpan.Parse(PlayerPrefs.GetString("TotalElapsedTime")) : TimeSpan.Zero;
        _startTime = DateTime.Now;
    }
    
    public TimeSpan GetElapsedTime()
    {
        return _elapsedTime;
    }
    
    public TimeSpan GetGameTime()
    {
        if (_isRunning)
        {
            return _gameTime + (DateTime.Now - _startTime);
        }
        else
        {
            return _gameTime;
        }
    }
    
    public bool IsNightMode()
    {
        return _isNightMode;
    }
}