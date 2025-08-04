using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")] [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;
    //[SerializeField] private bool startQuestAtInitialized = false;
    
    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private QuestIcon _questIcon;
    
    private void Awake()
    {
        questId = questInfoForPoint.id;
        _questIcon = GetComponentInChildren<QuestIcon>();
    }

    private void OnEnable()
    {
        GameEventsManagerSO.instance.questEvents.onQuestStateChange += QuestStateChanged;
        GameEventsManagerSO.instance.inputEvents.onSubmitPressed += SubmitPressed; 
    }
    
    private void OnDisable()
    {
        GameEventsManagerSO.instance.questEvents.onQuestStateChange -= QuestStateChanged;
        GameEventsManagerSO.instance.inputEvents.onSubmitPressed -= SubmitPressed;
    }

    private void SubmitPressed()
    {
        if (!playerIsNear)
        {
            return;
        }
        
        // start or finish a quest
        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            GameEventsManagerSO.instance.questEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            GameEventsManagerSO.instance.questEvents.FinishQuest(questId);
        }
    }

    // private void StartQuestAtInitialized()
    // {
    //     _questIcon.gameObject.SetActive(false);
    //     GameEventsManagerSO.instance.questEvents.StartQuest(questId);
    // }
    
    private void QuestStateChanged(Quest quest)
    {
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            _questIcon.SetState(currentQuestState, startPoint, finishPoint);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = true;
            GameEventsManagerSO.instance.miscEvents.ShowSubmitButton(true);
        }
    }
    
    private void OnTriggerExit2D (Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = false;
            GameEventsManagerSO.instance.miscEvents.ShowSubmitButton(false);
        }
    }
}
