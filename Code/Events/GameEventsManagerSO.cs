using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEventsManager", menuName = "Managers/GameEventsManager")]
public class GameEventsManagerSO : ScriptableObject
{
    /*
    // Game Events Manager Singleton
    public static GameEventsManagerSO instance { get; private set; }
    
    public InputEvents inputEvents;
    public PlayerEvents playerEvents;
    public ItemEvents itemEvents;
    //public MiscEvents miscEvents;
    public QuestEvents questEvents;

    private void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
            return;
        }
        instance = this;
    }
    */

    private static GameEventsManagerSO _instance;

    public static GameEventsManagerSO instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameEventsManagerSO>("GameEventsManager");
                if (_instance == null)
                {
                    Debug.LogError("GameEventsManager not found in Resources folder.");
                }
            }

            return _instance;
        }
    }
    
    public InputEvents inputEvents;
    public PlayerEvents playerEvents;
    public ItemEvents itemEvents;
    public QuestEvents questEvents;
    public MiscEvents miscEvents;
    public EnemiesEvent enemyEvents;
    
    private void OnEnable()
    {
        inputEvents = new InputEvents();
        playerEvents = new PlayerEvents();
        itemEvents = new ItemEvents();
        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
        enemyEvents = new EnemiesEvent();
    }
}
