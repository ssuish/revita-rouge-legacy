using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }
    [Header("Quest Info")] public string displayName;
    [Header("Requirements")] public int levelRequirement;
    public QuestInfoSO[] questPrerequisites;
    [Header("Steps")] public GameObject[] questStepPrefabs;
    [FormerlySerializedAs("goldReward")] [Header("Rewards")] public int itemReward; //rewards
    public int experienceRewards;
    [Header("Config")] 
    [SerializeField] public float timerInSeconds;
    [SerializeField] public bool isRepeatable;
    [SerializeField] public bool isTimed;
    [SerializeField] public bool isDaily;
    [SerializeField] public bool startQuestAtInitialized;
    
    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
