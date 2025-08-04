using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    private string questId;
    private int stepIndex;

    // ReSharper disable Unity.PerformanceAnalysis
    public void InitializeQuestStep(string questIdParam, int stepIndexParam, string questStepState)
    {
        this.questId = questIdParam;
        this.stepIndex = stepIndexParam;
        if (!string.IsNullOrEmpty(questStepState))
        {
            SetQuestStepState(questStepState);
        }
    }

    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;

            GameEventsManagerSO.instance.questEvents.AdvanceQuest(questId);

            Destroy(this.gameObject);
        }
    }
    
    protected void FinishQuestImmediately()
    {
        if (isFinished)
        {
            isFinished = true;
            GameEventsManagerSO.instance.questEvents.FinishQuest(questId);
            Destroy(this.gameObject);
        }
    }

    protected void ChangeState(string newState, string newStatus)
    {
        GameEventsManagerSO.instance.questEvents.QuestStepStateChanged(
            questId, 
            stepIndex, 
            new QuestStepState(newState, newStatus));
    }

    protected abstract void SetQuestStepState(string state);

    // To be deleted no longer in use.
    protected abstract void SetQuestStepStatus(string status);
}