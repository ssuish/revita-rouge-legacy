using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.SceneManager
{
    public class SceneProgressionManager : MonoBehaviour
    {
        public Leveling leveling;
        public InventoryController inventoryController;
        // inventory controller not set up yet on other areas. 
        
        private int _progressionCounter;
        private int _playerLevel;
        private LevelLoader _levelLoader;
        void Awake()
        {
            leveling = FindObjectOfType<Leveling>();
            
            if (leveling == null)
                Debug.LogError("Leveling component not found on any object in the scene.");
            else
                Debug.Log("Leveling component found on: " + leveling.gameObject.name);

            inventoryController = FindObjectOfType<InventoryController>();
            if (inventoryController == null)
                Debug.LogError("InventoryController not found in the scene.");
        }

        // Start is called before the first frame update
        void Start()
        {
            LoadScenesToList();
            _playerLevel = leveling.Level;
            _progressionCounter = SceneData.ProgressionCounter;
            
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "AutomataBase")
            {
                SceneData.ProgressionCounter = 0;
                _progressionCounter = 0;
            }
            
            _levelLoader = FindObjectOfType<LevelLoader>();
            
            Debug.Log("Progression Counter: " + _progressionCounter);
        }

        private void OnEnable()
        {
            GameEventsManagerSO.instance.miscEvents.OnSceneChangeTriggered += ProgressScene;
            GameEventsManagerSO.instance.playerEvents.onPlayerLevelChange += PlayerLevelChanged;
            // Add event to inventory handler
        }

        private void OnDisable()
        {
            GameEventsManagerSO.instance.miscEvents.OnSceneChangeTriggered -= ProgressScene;
            GameEventsManagerSO.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChanged;
            // Add event to inventory handler
        }

        private void PlayerLevelChanged(int level)
        {
            _playerLevel = level;
        }
        
        private async void ProgressScene()
        {
            try
            {
                // // Ending Scene is a special case. It will load the ending scene. NOT YET IMPLEMENTED
                // GameObject parent = transform.parent != null ? transform.parent.gameObject : null;;
                // if (parent != null)
                // {
                //     if (parent == GameObject.Find("Ending") || parent == GameObject.Find("EndingScene"))
                //     {
                //         UnityEngine.SceneManagement.SceneManager.LoadScene("Ending");
                //         //GameEventsManagerSO.instance.miscEvents.DialogueRequested(true, "Ending");
                //     }
                //     
                // }
                //Debug.Log("Current Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            
                inventoryController.SaveInventory();
                leveling.SaveLevelProgress();
        
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Tutorial")
                {
                    // Load the automata base scene
                    await LoadSceneAsync("AutomataBase");
                    return;
                }

                _progressionCounter++;
                SceneData.ProgressionCounter = _progressionCounter;

                string nextScene = GetNextScene();
                await LoadSceneAsync(nextScene);
            }
            catch (Exception e)
            {
                Debug.LogError("Error in ProgressScene: " + e.Message);
            }
        }
        
        private Task LoadSceneAsync(string sceneName)
        {
            var tcs = new TaskCompletionSource<bool>();
            _levelLoader.LoadLevel();
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            if (asyncOperation != null) 
                asyncOperation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
        
        private string GetNextScene()
        {
            switch (_progressionCounter)
            {
                case 5:
                    return GetRandomScene(SceneData.Area1BossZones);
                case > 5 when _progressionCounter % 5 == 0 && _playerLevel < 10:
                    return GetRandomScene(SceneData.Area1BossZones);
                case > 5 and < 10 when _playerLevel >= 10:
                    return GetRandomScene(SceneData.Area2NormalZones);
                case 10 when _playerLevel >= 10:
                    return GetRandomScene(SceneData.Area2BossZones);
                case > 10 and < 15 when _playerLevel >= 15:
                    return GetRandomScene(SceneData.Area3NormalZones);
                case 15 when _playerLevel >= 15:
                    return SceneData.Area3BossZones[0];
                case 16:
                    _progressionCounter = 0;
                    SceneData.ProgressionCounter = 0;
                    return "AutomataBase";
                default:
                    return GetRandomScene(SceneData.Area1NormalZones);
            }
        }

        private static void LoadScenesToList()
        {
            // Load scenes from Scene folder
            for (int i = 1; i < 7; i++)
            {
                SceneData.Area1NormalZones.Add("Area1A" + i);
                SceneData.Area2NormalZones.Add("Area2A" + i);
                SceneData.Area3NormalZones.Add("Area3A" + i);
            }

            for (int i = 1; i < 3; i++)
            {
                SceneData.Area1BossZones.Add("Area1B" + i);
                SceneData.Area2BossZones.Add("Area2B" + i);
            }

            SceneData.Area3BossZones.Add("Area3B1");
        }

        private string GetRandomScene(List<string> sceneList)
        {
            string selectedScene = sceneList[Random.Range(0, sceneList.Count)];

            // Ensure that the next scene isn't the same as the current scene
            while (selectedScene == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                selectedScene = sceneList[Random.Range(0, sceneList.Count)];
            }

            return selectedScene;
        }
    }
}