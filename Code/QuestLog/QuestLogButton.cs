using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class QuestLogButton : MonoBehaviour, ISelectHandler
{
    public Button button { get; private set; }
    private UnityAction onSelectAction;
    private TextMeshProUGUI buttonText; // Quest title

    public void Initialized(string displayName, UnityAction selectAction)
    {
        this.button = this.GetComponent<Button>();
        this.buttonText = this.GetComponentInChildren<TextMeshProUGUI>();
        this.buttonText.text = displayName;
        this.onSelectAction = selectAction;
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelectAction();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void SetState(QuestState state)
    {
        switch(state)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                buttonText.color = Color.grey;
                break;
            case QuestState.CAN_START:
                buttonText.color = Color.blue;
                break;
            case QuestState.IN_PROGRESS:
                buttonText.color = Color.black;
                break;
            case QuestState.CAN_FINISH:
                buttonText.color = Color.yellow;
                break;
            case QuestState.FINISHED:
                buttonText.color = Color.green;
                break;
            default:
                Debug.LogError("Unknown QuestState: " + state);
                break;
        }
    }
}