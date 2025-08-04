using UnityEngine;

public class Quest 
{
    // static info
    public QuestInfoSO info;
    
    // state info
    public QuestState state;
    private int currentQuestStepIndex;
    private readonly QuestStepState[] _questStepStates;
    public float timeRemaining = 0;

    public Quest(QuestInfoSO questInfo)
    {
        info = questInfo;
        state = QuestState.REQUIREMENTS_NOT_MET;
        currentQuestStepIndex = 0;
        _questStepStates = new QuestStepState[info.questStepPrefabs.Length];
        for (int i = 0; i < _questStepStates.Length; i++)
        {
            _questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestInfoSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)//, float timeRemaining
    {
        info = questInfo;
        state = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        _questStepStates = questStepStates;
        //this.timeRemaining = timeRemaining;
        
        // if the quest step states and prefabs are different lengths.
        // something has changed during development and the saved data is out of sync.
        if (_questStepStates.Length != info.questStepPrefabs.Length)
        {
            Debug.LogWarning("Quest step states and prefabs are different lengths. Its out of sync. QuestID=" + info.id);
        }
    }

    public void MovetoNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < info.questStepPrefabs.Length);
    }
    
    public void InstantiateQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            QuestStep questStep = Object.Instantiate(questStepPrefab, parentTransform)
                .GetComponent<QuestStep>();
            questStep.InitializeQuestStep(info.id, currentQuestStepIndex, _questStepStates[currentQuestStepIndex].state);
        }
        else if (CurrentStepExists())
        {
            Debug.LogWarning("Tried to instantiate quest step, but questStepPrefab was null: QuestID=" + info.id +
                             " StepIndex=" + currentQuestStepIndex);
        }
    }
    
    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists())
        {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that " +
                             "there's no current step: QuestID=" + info.id + " StepIndex=" + currentQuestStepIndex);
        }

        return questStepPrefab;
    }
    
    public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
    {
        if (stepIndex < _questStepStates.Length)
        {
            _questStepStates[stepIndex].state = questStepState.state;
            _questStepStates[stepIndex].status = questStepState.status;
        }
        else
        {
            Debug.LogWarning("Tried to store quest step state, but stepIndex was out of range: QuestID=" + info.id +
                             " StepIndex=" + stepIndex);
        }
    } 
    
    public QuestData GetQuestData()
    {
        return new QuestData(state, currentQuestStepIndex, _questStepStates);
    }

    public string GetFullStatusText()
    {
        string fullStatusText = "";

        if (state == QuestState.REQUIREMENTS_NOT_MET)
        {
            fullStatusText = "Requirements not met to start quest";
        }
        else if (state == QuestState.CAN_START)
        {
            fullStatusText = "Ready to start quest";
        }
        else
        {
            //Debug.Log("QuestID=" + info.id + " State=" + state + " CurrentStepIndex=" + currentQuestStepIndex);
            for (int i = 0; i < currentQuestStepIndex; i++)
            {
                fullStatusText += "<s>" + _questStepStates[i].status + "</s>\n";
            }

            if (CurrentStepExists())
            {
                fullStatusText += "<b>" + _questStepStates[currentQuestStepIndex].status + "</b>\n";
            }

            if (state == QuestState.CAN_FINISH)
            {
                fullStatusText += "\nThe Quest is ready to be turned in.";
            }
            else if (state == QuestState.FINISHED)
            {
                fullStatusText += "\nThe Quest has been completed.";
            }
        }
        
        return fullStatusText;
    }

    public void ResetQuestStep()
    {
        currentQuestStepIndex = 0;
    }
    
    // TODO - time related methods
    /*public void UpdateTime(float deltaTime)
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                //ChangeQuestState(info.id, QuestState.FAILED);
            }
        }
    }*/
}
