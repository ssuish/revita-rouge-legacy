using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Config")] [SerializeField] private bool loadQuestState = true;
    [SerializeField] private WorldTime worldTime;

    private Dictionary<string, Quest> questMap;

    // Quest start requirements
    private int currentPlayerLevel;
    private float oldTime;
    private bool _isOnline; // Default to online mode
    public IEnumerable<Quest> Quests => questMap.Values;
    
    private static QuestManager _instance;
    
    public static QuestManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<QuestManager>();
            }

            return _instance;
        }
    }
    
    private void Awake()
    {
        questMap = CreateQuestsMap();

        //Quest quest = GetQuestByID("CollectCoinQuest");
        //Debug.Log(quest.info.displayName);
        //Debug.Log(quest.info.levelRequirement);
        //Debug.Log(quest.state);
        //Debug.Log(quest.CurrentStepExists());
    }

    private void OnEnable()
    {
        if (GameEventsManagerSO.instance == null)
        {
            Debug.LogError("GameEventsManagerSO instance is not set.");
            return;
        }

        GameEventsManagerSO.instance.questEvents.onStartQuest += StartQuest;
        GameEventsManagerSO.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManagerSO.instance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManagerSO.instance.questEvents.onQuestStepStateChange += QuestStepStateChanged;

        GameEventsManagerSO.instance.playerEvents.onPlayerLevelChange += PlayerLevelChanged;

        worldTime.WorldTimeChanged += OnWorldTimeChanged;
        GameEventsManagerSO.instance.miscEvents.OnSceneChangeTriggered += OnSceneLoaded;
        GameEventsManagerSO.instance.miscEvents.OnSaveQuest += SaveQuest;
    }

    private void Start()
    {
        //broadcast the initial quest state
        foreach (Quest quest in questMap.Values)
        {
            //initialize any loaded quest steps
            if (quest.info.startQuestAtInitialized &&
                quest.state == QuestState.REQUIREMENTS_NOT_MET &&
                CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
            }

            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateQuestStep(this.transform);
            }

            oldTime = quest.info.timerInSeconds;

            GameEventsManagerSO.instance.questEvents.QuestStateChanged(quest);
        }
    }

    private void OnDisable()
    {
        GameEventsManagerSO.instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManagerSO.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManagerSO.instance.questEvents.onFinishQuest -= FinishQuest;

        GameEventsManagerSO.instance.questEvents.onQuestStepStateChange -= QuestStepStateChanged;

        GameEventsManagerSO.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChanged;

        worldTime.WorldTimeChanged += OnWorldTimeChanged;
        GameEventsManagerSO.instance.miscEvents.OnSceneChangeTriggered -= OnSceneLoaded;
        GameEventsManagerSO.instance.miscEvents.OnSaveQuest -= SaveQuest;
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestByID(id);
        quest.state = state;
        GameEventsManagerSO.instance.questEvents.QuestStateChanged(quest);
    }

    // Check if the player level is high enough to start the quest
    private void PlayerLevelChanged(int level)
    {
        currentPlayerLevel = level;
    }

    private void OnWorldTimeChanged(object sender, TimeSpan realWorldTime)
    {
        TimeSpan elapsedTime;

        if (worldTime.isOfflineMode)
        {
            // Get the in-game offline time if in offline mode
            elapsedTime = DateTime.Now - worldTime.GetLastUpdateTime();
            _isOnline = false;
        }
        else
        {
            // Get the real-world time if online mode
            elapsedTime = worldTime.GetLastUpdateTime().TimeOfDay;
            _isOnline = true;
        }

        if (elapsedTime.TotalSeconds < 0)
        {
            elapsedTime = TimeSpan.Zero;
        }

        // Decrease the time machine time by the elapsed time
        //float timer = GetQuestByID(questInfo.id).info.timerInSeconds;
        //timer -= (float)elapsedTime.TotalSeconds;
        //GetQuestByID(questInfo.id).info.timerInSeconds = timer;
        //Debug.Log("Quest Info id: " + questInfo.id + " Timer: " + timer);

        // Loop through all quests
        foreach (Quest quest in questMap.Values)
        {
            // If the quest is in progress
            if (quest.state == QuestState.IN_PROGRESS && quest.info.isTimed)
            {
                quest.info.timerInSeconds--;
                quest.timeRemaining = quest.info.timerInSeconds;

                // If the quest timer is less than or equal to 0
                if (quest.info.timerInSeconds <= 0)
                {
                    // Restart the quest
                    RestartQuest(quest.info.id);
                    Debug.Log("Quest " + quest.info.id + " has expired.");
                }
            }

            // If the quest is done reset the timer
            if (quest.state == QuestState.FINISHED)
            {
                if (quest.info.isDaily)
                {
                    // Change the quest state to requirements not met after midnight
                    if (elapsedTime.Equals(TimeSpan.Zero))
                    {
                        quest.ResetQuestStep();
                        StartQuest(quest.info.id);
                        RestartQuest(quest.info.id);
                        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
                    }
                }
                else if (quest.info.isRepeatable)
                {
                    quest.ResetQuestStep();
                    StartQuest(quest.info.id);
                    RestartQuest(quest.info.id);
                }
            }
        }

        // Update the quest timer
        worldTime.SetLastUpdateTime(DateTime.Now);
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        // start true and prove to be false
        if (currentPlayerLevel < quest.info.levelRequirement)
        {
            return false;
        }

        // player level requirement

        // check quest prerequisites for completion
        foreach (QuestInfoSO prerequisiteQuestInfoSo in quest.info.questPrerequisites)
        {
            if (GetQuestByID(prerequisiteQuestInfoSo.id).state != QuestState.FINISHED)
            {
                return false;
            }
        }

        return true;
    }

    private void Update()
    {
        //loop through all quests
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }

            if (quest.info.isRepeatable && quest.state == QuestState.FINISHED && !quest.info.isDaily)
            {
                ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
            }
            //if (GetQuestByID(questInfo.id) != null)
            //{
            /*if (GetQuestByID(questInfo.id).info.timerInSeconds <= 0)
            {
                ChangeQuestState(questInfo.id, QuestState.REQUIREMENTS_NOT_MET);
            }*/
            //}
        }
    }

    private void RestartQuest(string id)
    {
        Quest quest = GetQuestByID(id);
        quest.info.timerInSeconds = oldTime;
        ChangeQuestState(id, QuestState.REQUIREMENTS_NOT_MET);
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestByID(id);
        quest.InstantiateQuestStep(this.transform);
        ChangeQuestState(id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestByID(id);

        //move on the next step
        quest.MovetoNextStep();

        //if there are more steps. instantiate the next one
        if (quest.CurrentStepExists())
        {
            quest.InstantiateQuestStep(this.transform);
        }
        // if there are no more steps, finish the quest
        else
        {
            ClaimRewards(quest);
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestByID(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
    }

    private void ClaimRewards(Quest quest)
    {
        GameEventsManagerSO.instance.playerEvents.ExperienceGained(quest.info.experienceRewards); // use struct for compounds
    }

    private void QuestStepStateChanged(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestByID(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        GameEventsManagerSO.instance.questEvents.QuestStateChanged(quest);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestsMap()
    {
        // Targets ../Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();

        // Check if any quests were loaded
        if (allQuests == null || allQuests.Length == 0)
        {
            Debug.LogError("No quests found in Resources/Quests folder.");
            return idToQuestMap;
        }

        // Create the quest map
        foreach (QuestInfoSO questInfo in allQuests)
        {
            /*if (questInfo == null)
            {
                Debug.LogWarning("QuestInfoSO is null");
                continue;
            }

            if (string.IsNullOrEmpty(questInfo.id))
            {
                Debug.LogWarning("QuestInfoSO ID is null or empty");
                continue;
            }*/

            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogError("Duplicate ID found when creating quest map: " + questInfo.id);
            }

            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }

        return idToQuestMap;
    }

    private Quest GetQuestByID(string id)
    {
        //Quest quest = questMap[id];
        if (questMap.TryGetValue(id, out Quest quest))
        {
            return quest;
        }

        Debug.LogError("Quest not found in quest map: " + id);
        return null;
    }

    private void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest) // only active quest refactor
    {
        try
        {
            if (_isOnline)
            {
                //Debug.Log("Saving quest: " + quest.info.id);
                QuestData questData = quest.GetQuestData();
                // Check if the JSON Utility works
                string serializedData = JsonUtility.ToJson(questData);
                //Debug.Log("Serialized data for quest " + serializedData);
                // PlayerPrefs are not ideal for long term, use Save & Load System here.
                PlayerPrefs.SetString(quest.info.id, serializedData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving quest: " + quest.info.id + " " + e.Message);
            throw;
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;

        try
        {
            // PlayerPrefs are not ideal for long term, use Save & Load System here.
            // load quest from saved data
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            {
                //Debug.Log("Loading quest: " + questInfo.id);
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                //Debug.Log("Serialized data for quest " + serializedData);
                if (questData == null)
                {
                    Debug.LogError("Failed to deserialize quest data for quest: " + questInfo.id);
                }
                else
                {
                    quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
                }
            }

            // else, create new quest
            // if (quest == null)
            // {
            //     quest = new Quest(questInfo);
            // }
            quest ??= new Quest(questInfo);
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading quest: " + questInfo.id + " " + e.Message);
            throw;
        }

        return quest;
    }

    private void OnSceneLoaded()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }
    
    private void SaveQuest()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }
}