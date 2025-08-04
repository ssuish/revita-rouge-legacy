using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class Status : MonoBehaviour
{
    [SerializeField] private List<Sprite> statusIcons = new();
    [SerializeField] private TextMeshProUGUI internetStatusText;
    [SerializeField] private GameObject dayNightCycleObject;
    
    private QuestTimer _questTimer;
    private int _currentIconIndex;
    private int _spriteIndexSet;
    private Image statusIcon;
    private TimeSpan _elapsedTime;
    
    private void OnEnable()
    {
        GameEventsManagerSO.instance.miscEvents.OnInternetStatusChange += InternetStatusChange;
    }
    
    private void OnDisable()
    {
        GameEventsManagerSO.instance.miscEvents.OnInternetStatusChange -= InternetStatusChange;
    }

    private void Start()
    {
        _questTimer = FindObjectOfType<QuestTimer>();

        if (_questTimer is null)
        {
            Debug.LogError("Quest Timer not found in the scene.");
        }
        
        statusIcon = dayNightCycleObject.GetComponent<Image>();
        
        if (statusIcon is null)
        {
            Debug.LogError("Image component not found in the children of the Status object.");
        }
    }

    private void FixedUpdate()
    {
        UpdateCurrentIconIndex();

        if (_spriteIndexSet == _currentIconIndex) return;
        
        UpdateIconSprite();
    }

    private void InternetStatusChange(bool isConnected)
    {
        internetStatusText.text = isConnected ? "Playing Online Mode" : "Playing Offline Mode";
    }
    
    private void UpdateCurrentIconIndex()
    {
        _elapsedTime = _questTimer.GetElapsedTime();
        
        //Debug.Log("Elapsed Time: " + _elapsedTime);
        _currentIconIndex = _elapsedTime.Hours switch
        {
            // Check if the elapsed time is day time, morning, early night, or late night
            >= 6 and < 12 => 0,
            >= 12 and < 18 => 1,
            >= 18 and < 24 => 2,
            _ => 3
        };
    }
    
    private void UpdateIconSprite()
    {
        _spriteIndexSet = _currentIconIndex;
        statusIcon.sprite = statusIcons[_currentIconIndex];
    }
}
