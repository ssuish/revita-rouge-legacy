using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestProgressionLogUI : MonoBehaviour
{
    [SerializeField] private GameObject contentParent;
    [SerializeField] private GameObject TabsBut;
    
    [SerializeField] private GameObject questProgressionCheckList;
    [SerializeField] private GameObject endingButton;
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private TextMeshProUGUI environmentTextObject;
    [SerializeField] private TextMeshProUGUI questTimeText;
    [SerializeField] private TMP_FontAsset notoSansFontAsset;
    [SerializeField] private List<string> environmentTexts = new();
    [SerializeField] private GameObject questProgressionTextPrefab;
    [SerializeField] private Sprite questProgressionImagePrefab;
    
    public int _questDoneCount;
    private readonly List<GameObject> _questProgressionTexts = new();
    private readonly HashSet<string> _finishedQuests = new();

    // private void FixedUpdate()
    // {
    //     ShowTotalGameTime();
    // }

    private void OnEnable()
    {
        GameEventsManagerSO.instance.inputEvents.onQuestProgressionTogglePressed += QuestProgressionTogglePressed;
        GameEventsManagerSO.instance.inputEvents.onEndingTogglePressed += EndingTogglePressed;
        GameEventsManagerSO.instance.questEvents.onQuestStateChange += QuestStateChanged;
        
        // Check if other UI elements are active
        GameEventsManagerSO.instance.miscEvents.OnUIOpened += HandleUIOpened;
    }
    
    private void OnDisable()
    {
        GameEventsManagerSO.instance.inputEvents.onQuestProgressionTogglePressed -= QuestProgressionTogglePressed;
        GameEventsManagerSO.instance.inputEvents.onEndingTogglePressed -= EndingTogglePressed;
        GameEventsManagerSO.instance.questEvents.onQuestStateChange -= QuestStateChanged;
        
        // Save the questDoneCount state
        //PlayerPrefs.SetInt("QuestDoneCount", _questDoneCount);
        //PlayerPrefs.Save();
        
        // Check if other UI elements are active
        GameEventsManagerSO.instance.miscEvents.OnUIOpened -= HandleUIOpened;
    }

    private void Start()
    {
        // Load the questDoneCount state
        //_questDoneCount = PlayerPrefs.GetInt("QuestDoneCount", 0);
        _questDoneCount = 0;
        
        foreach (Quest quest in QuestManager.Instance.Quests)
        {
            if (!quest.info.name.Contains("Chapter")) continue;

            GameObject questProgressionText = CreateTextMeshPro(quest.info.id, AddQuestInfo(quest), questProgressionCheckList.transform);
            _questProgressionTexts.Add(questProgressionText);
            
            CheckQuestStateIfNotifiable(quest, questProgressionText);
        }

        HideUI();
        StartCoroutine(UpdateTotalGameTime());
        UpdateProgressBar();
    }

    private void CheckQuestStateIfNotifiable(Quest quest, GameObject questProgressionText)
    {
        switch (quest.state)
        {
            case QuestState.CAN_START or QuestState.CAN_FINISH:
                questProgressionText.GetComponentInChildren<QuestNotifManager>().ShowQuestNotification();
                break;
                
            // Check if the quest is finished and update the sprite
            case QuestState.FINISHED:
            {
                var questImage = questProgressionText.GetComponentInChildren<Image>();
                questImage.sprite = questProgressionImagePrefab;
                questImage.color = Color.green;
                questProgressionText.GetComponentInChildren<QuestNotifManager>().HideQuestNotification();
                
                if (!_finishedQuests.Contains(quest.info.id))
                    if (_questDoneCount < _questProgressionTexts.Count)
                    {
                        _finishedQuests.Add(quest.info.id);
                        _questDoneCount++;
                    }

                break;
            }
                
            default:
                questProgressionText.GetComponentInChildren<QuestNotifManager>().HideQuestNotification();
                break;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void QuestStateChanged(Quest quest)
    {
        var questID = quest.info.id;

        if (!quest.info.name.Contains("Chapter")) return;

        Transform questTransform = questProgressionCheckList.transform.Find(questID);

        if (questTransform != null)
        {
            TextMeshProUGUI textMesh = questTransform.GetComponentInChildren<TextMeshProUGUI>();

            if (textMesh != null)
            {
                textMesh.text = AddQuestInfo(quest);
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in questTransform: " + questID);
            }

            CheckQuestStateIfNotifiable(quest, questTransform.gameObject);
            
            UpdateProgressBar();
        }
    }

    private string AddQuestInfo(Quest quest)
    {
        string questName;
            
        questName = quest.state == QuestState.FINISHED ? $"<color=red><s>{quest.info.displayName}</s></color>" : quest.info.displayName;
            
        QuestState questState = quest.state;
        string statusText = GetStatusText(questState);
        string questStatus = $"<b>{questName}</b><br>{statusText}";

        return questStatus;
    }

    private void QuestProgressionTogglePressed()
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
        GameEventsManagerSO.instance.miscEvents.UIOpened(this.GetType().Name);
        RandomEnvironmentText();
    }
    
    private void HideUI()
    {
        contentParent.SetActive(false);
    }
    
    private GameObject CreateTextMeshPro(string title, string text, Transform parent)
    {
        // Instantiate the prefab
        GameObject textObject = Instantiate(questProgressionTextPrefab, parent);

        // Set the name of the instantiated object
        textObject.name = title;

        // Find and modify the TextMeshProUGUI component
        TextMeshProUGUI textMesh = textObject.GetComponentInChildren<TextMeshProUGUI>();
        
        if (textMesh == null) return textObject;
        
        textMesh.font = notoSansFontAsset;
        textMesh.text = text;
        textMesh.fontSize = 9;
        textMesh.color = Color.black;
        textMesh.alignment = TextAlignmentOptions.Left;
        
        return textObject;
    }
    
    private string GetStatusText(QuestState questState)
    {
        switch (questState)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                return "Requirements for this quest are not met.";
            case QuestState.CAN_START:
                return "Quest is available, you can start it now.";
            case QuestState.IN_PROGRESS:
                return "Quest is in progress.";
            case QuestState.CAN_FINISH:
                return "Quest is ready to be finished.";
            case QuestState.FINISHED:
                return "Finished! Well Done!";
            default:
                return "Unknown State";
        }
    }
    
    private void UpdateProgressBar()
    {
        int totalQuests = _questProgressionTexts.Count;
        
        progressBar.min = 0;
        progressBar.max = totalQuests;

        progressBar.currentProgress = _questDoneCount;
    }
    
    private void RandomEnvironmentText()
    {
        int randomIndex = UnityEngine.Random.Range(0, environmentTexts.Count);
        
        if (_questDoneCount == _questProgressionTexts.Count)
        {
            environmentTextObject.text = "<b>All quests are done!<br>You finished the game!</b>";
            // show ending button
            endingButton.SetActive(true);
            return;
        }
        
        environmentTextObject.text = environmentTexts[randomIndex];
    }
    
    private void EndingTogglePressed()
    {
        if (endingButton.activeInHierarchy)
        {
            SceneManager.LoadScene("Ending");
        }
    }
    
    private IEnumerator UpdateTotalGameTime()
    {
        while (true)
        {
            ShowTotalGameTime();
            yield return new WaitForSeconds(30); // Wait for 60 seconds
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void ShowTotalGameTime()
    {
        //Debug.Log("Showing total game time...");
        QuestTimer questTimer = FindObjectOfType<QuestTimer>();
        TimeSpan totalTime = questTimer.GetGameTime();
        
        int days = Math.Abs(totalTime.Days);
        int hours = Math.Abs(totalTime.Hours);
        int minutes = Math.Abs(totalTime.Minutes);
        
        questTimeText.text = $"Total time played:<br>{days} days {hours} hours " +
                             $"{minutes} minutes";
    }
}
