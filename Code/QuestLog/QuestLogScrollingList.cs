using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestLogScrollingList : MonoBehaviour
{
    [Header("Component")] [SerializeField] private GameObject contentParent;

    [Header("Quest Log Action")] [SerializeField]
    private GameObject questLogButtonPrefab;

    private readonly Dictionary<string, QuestLogButton> _idToButtonMap = new Dictionary<string, QuestLogButton>();

    private void Start()
    {   
        /* Testing
        for (int i = 0; i < 10; i++)
        {
            QuestInfoSO questInfoSoTest = ScriptableObject.CreateInstance<QuestInfoSO>();
            questInfoSoTest.id = "test " + i;
            questInfoSoTest.displayName = "Test Quest " + i;
            questInfoSoTest.questStepPrefabs = new GameObject[0];
            Quest quest = new Quest(questInfoSoTest);

            QuestLogButton questLogButton = CreateButtonIfNotExists(quest, () =>
            {
                Debug.Log("Selected " + quest.info.displayName);
            });

            if (i == 0)
            {
                questLogButton.button.Select();
            }
        }
        */
        
        
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public QuestLogButton CreateButtonIfNotExists(Quest quest, UnityAction selectAction)
    {
        QuestLogButton questLogButton;
    
        if (!_idToButtonMap.TryGetValue(quest.info.id, out var value))
        {
            questLogButton = InstantiateQuestLogButton(quest, selectAction);
        }
        else
        {
            questLogButton = value;
        }
        
        return questLogButton;
    }
    
    public void RemoveButton(string id)
    {
        if (_idToButtonMap.ContainsKey(id))
        {
            Destroy(_idToButtonMap[id].gameObject);
            _idToButtonMap.Remove(id);
        }
    }

    private QuestLogButton InstantiateQuestLogButton(Quest quest, UnityAction selectAction)
    {
        QuestLogButton questLogButton = Instantiate(
            questLogButtonPrefab,
            contentParent.transform).GetComponent<QuestLogButton>();
        questLogButton.gameObject.name = quest.info.id + "_button";
        questLogButton.Initialized(quest.info.displayName, selectAction);
        _idToButtonMap[quest.info.id] = questLogButton;
        return questLogButton;
    }
}