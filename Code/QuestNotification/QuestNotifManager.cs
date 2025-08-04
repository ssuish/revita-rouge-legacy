using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNotifManager : MonoBehaviour
{
    [SerializeField] private GameObject questNotificationParent;
    public bool isUIQuestButton;
    public string questName;
    private bool _isQuestNotificationActive;

    private void Awake()
    {
        GameEventsManagerSO.instance.questEvents.onQuestStateChange += QuestStateChanged;
    }

    private void OnDestroy()
    {
        GameEventsManagerSO.instance.questEvents.onQuestStateChange -= QuestStateChanged;
    }
    
    private void QuestStateChanged(Quest quest)
    {
        if (quest.info.id.Contains("Chapter") == false) return;

        if (quest.state is QuestState.CAN_START or QuestState.CAN_FINISH)
        {
            ShowQuestNotification();
        }
    }
    
    public void ShowQuestNotification()
    {
        questNotificationParent.SetActive(true);
        _isQuestNotificationActive = true;
    }

    public void HideQuestNotification()
    {
        questNotificationParent.SetActive(false);
        _isQuestNotificationActive = false;
    }
}
