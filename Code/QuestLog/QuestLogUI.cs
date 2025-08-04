using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class QuestLogUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent; 
    [SerializeField] private GameObject TabsBut;


    [SerializeField] private QuestLogScrollingList scrollingList;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questDisplayNameText;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private TextMeshProUGUI experienceRewardText;
    [SerializeField] private TextMeshProUGUI levelRequirementsText;
    [SerializeField] private TextMeshProUGUI questRequirementsText;
    [SerializeField] private TextMeshProUGUI questTimerText;

    private Button firstSelectedButton;
    
    private void OnEnable()
    {
        GameEventsManagerSO.instance.inputEvents.onQuestLogTogglePressed += QuestLogTogglePressed;
        GameEventsManagerSO.instance.questEvents.onQuestStateChange += QuestStateChanged;
        
        // Check if other UI elements are active
        GameEventsManagerSO.instance.miscEvents.OnUIOpened += HandleUIOpened;
    }
    
    private void OnDisable()
    {
        GameEventsManagerSO.instance.inputEvents.onQuestLogTogglePressed -= QuestLogTogglePressed;
        GameEventsManagerSO.instance.questEvents.onQuestStateChange -= QuestStateChanged;
        GameEventsManagerSO.instance.miscEvents.OnUIOpened -= HandleUIOpened;
    }

    private void QuestLogTogglePressed()
    {
        if (contentParent.activeInHierarchy)
        {
            HideUI();
            TabsBut.SetActive(true);
        }
        else
        {
            ShowUI();        
            TabsBut.SetActive(false);

        }
    }
    
    // Check if other UI elements are active
    private void HandleUIOpened(string uiName)
    {
        if (uiName != this.GetType().Name && contentParent.activeInHierarchy)
        {
            HideUI();
        }
    }

    private void ShowUI()
    {
        contentParent.SetActive(true);
        // Updates the UI event
        GameEventsManagerSO.instance.miscEvents.UIOpened(this.GetType().Name);
        if (firstSelectedButton)
        {
            firstSelectedButton.Select();
        }
    }

    private void HideUI()
    {
        contentParent.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    private void QuestStateChanged(Quest quest)
    {
        QuestLogButton questLogButton = scrollingList.CreateButtonIfNotExists(quest, () =>
        {
            SetQuestLogInfo(quest);
        });
        
        // set first selected button
        if (firstSelectedButton == null)
        {
            firstSelectedButton = questLogButton.button;
        }
        
        questLogButton.SetState(quest.state);
        
        if (quest.state == QuestState.FINISHED && quest.info.isDaily)
        {
            scrollingList.RemoveButton(quest.info.id);
        }
    }
    
    private void SetQuestLogInfo(Quest quest)
    {
        questDisplayNameText.text = quest.info.displayName;
        
        // status
        questStatusText.text = quest.GetFullStatusText();
        
        // requirements
        levelRequirementsText.text = "Required LVL: " + quest.info.levelRequirement;
        levelRequirementsText.text += "\n";
        
        // timer
        TimeSpan time = TimeSpan.FromSeconds(quest.info.timerInSeconds);
        
        //questTimerText.text = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        
        questRequirementsText.text = "";
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            questRequirementsText.text += prerequisiteQuestInfo.displayName + "\n";
        }
        
        // rewards
        experienceRewardText.text = quest.info.experienceRewards + " XP";
    }
}
