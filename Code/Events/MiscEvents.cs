using UnityEngine;
using System;
using System.Collections.Generic;

public class MiscEvents
{
    public event Action<string, int> onSetTimer;
    public void SetTimer(string timerName, int time)
    {
        if (onSetTimer != null)
        {
            onSetTimer(timerName, time);
        }
    }

    public event Action<string> onTimerEnded;
    public void TimerEnded(string timerName)
    {
        if (onTimerEnded != null)
        {
            onTimerEnded(timerName);
        }
    }
    
    public event Action<bool> onNightTime;
    public void NightTime(bool isNightTime)
    {
        if (onNightTime != null)
        {
            onNightTime(isNightTime);
        }
    } 
    
    public event Action onDayPassed;
    public void DayPassed()
    {
        if (onDayPassed != null)
        {
            onDayPassed();
        }
    }

    public event Action<int, bool, bool, bool> onWayPointGateClosed;
    public void WaypointGateClosed(int index, bool isBefore, bool disableLineLoop, bool disableLooping)
    {
        if (onWayPointGateClosed != null)
        {
            onWayPointGateClosed(index, isBefore, disableLineLoop, disableLooping);
        }
    }

    public event Action OnGateClosedStopBoss;
    public void GateClosedStopBoss()
    {
        if (OnGateClosedStopBoss != null)
        {
            OnGateClosedStopBoss();
        }
    }
    
    public event Action OnSceneChangeTriggered;
    public void SceneChangeTriggered()
    {
        if (OnSceneChangeTriggered != null)
        {
            OnSceneChangeTriggered();
        }
    }

    public event Action<List<string>> OnGetItemInfo;
    public void GetItemInfo(List<string> items)
    {
        Debug.Log("GetItemInfo called with items count: " + items.Count);
        Debug.Log("onGetItemInfo is null: " + (OnGetItemInfo == null));
        if (OnGetItemInfo != null)
        { 
            OnGetItemInfo(items);
        }
    }
    
    public event Action OnBoss3Defeated;
    // ReSharper disable Unity.PerformanceAnalysis
    public void Boss3Defeated()
    {
        if (OnBoss3Defeated != null)
        {
            OnBoss3Defeated();
        }
    }
    
    public event Action OnAutomataUpgraded;
    // ReSharper disable Unity.PerformanceAnalysis
    public void AutomataUpgraded()
    {
        if (OnAutomataUpgraded != null)
        {
            OnAutomataUpgraded();
        }
    }
    
    public event Action<bool, string> OnDialogueRequested;
    // ReSharper disable Unity.PerformanceAnalysis
    public void DialogueRequested(bool isStart, string dialogueName)
    {
        OnDialogueRequested?.Invoke(isStart, dialogueName);
    }
    
    public event Action OnDialogueEnded;
    public void DialogueEnded()
    {
        OnDialogueEnded?.Invoke();
    }

    public event Action<bool> OnUIShowSubmitButton;
    public void ShowSubmitButton(bool isEnabled)
    {
        OnUIShowSubmitButton?.Invoke(isEnabled);
    }
    
    public event Action OnSaveQuest;
    // ReSharper disable Unity.PerformanceAnalysis
    public void SaveQuest()
    {
        OnSaveQuest?.Invoke();
    }
    
    public event Action<bool> OnInternetStatusChange;
    public void InternetStatusChange(bool isConnected)
    {
        OnInternetStatusChange?.Invoke(isConnected);
    }
    
    // Check if other UI elements are active
    public event Action<string> OnUIOpened;
    public void UIOpened(string uiName)
    {
        OnUIOpened?.Invoke(uiName);
    }
}
