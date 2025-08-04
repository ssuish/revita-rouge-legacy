using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class Leveling : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI LevelText;
    [SerializeField] public TextMeshProUGUI ExperienceText;
    [SerializeField] private float TargetXp;
    [SerializeField] private Image XpProgressBar;
    
    public int Level; 
    public float CurrentXp;
    //private string filePath;
    private bool isDirty = false;
    
    private const float XP_INCREMENT = 50f;

    private void OnEnable()
    {
        GameEventsManagerSO.instance.playerEvents.onExperienceGained += ExperienceController;
    }
    
    private void OnDisable()
    {
        GameEventsManagerSO.instance.playerEvents.onExperienceGained -= ExperienceController;
    }

    private void Start()
    {
        // Load the level and experience data at the start of the game
        LoadLevelProgress(); 
    }

    private void Update()
    {
        if (isDirty)
        {
            UpdateUI();
            isDirty = false;
        }
    }
    
    private void OnApplicationQuit()
    {
        // Automatically save the level progress when the game is closed
        SaveLevelProgress();
        //mainMenuHolder.MarkIntroductionComplete();
    }

    /*public void SaveLevelProgress()
    {
        string directoryPath = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using StreamWriter sw = new StreamWriter(filePath);
        sw.WriteLine(Level);
        sw.WriteLine(CurrentXp);
        sw.WriteLine(TargetXp);
    }

    public void LoadLevelProgress()
    {
        if (!File.Exists(filePath))
        {
            //Debug.Log("Save file not found, starting at default level.");
            Level = 0;
            CurrentXp = 0;
            return;
        }

        using StreamReader sr = new StreamReader(filePath);
        if (int.TryParse(sr.ReadLine(), out int loadedLevel))
        {
            Level = loadedLevel;
            //Debug.Log("Loaded Level: " + Level);
            GameEventsManagerSO.instance.playerEvents.PlayerLevelChange(Level);
        }
        else
        {
            //Debug.LogError("Failed to load Level from save file.");
        }

        if (float.TryParse(sr.ReadLine(), out float loadedXp))
        {
            CurrentXp = loadedXp;
            //Debug.Log("Loaded CurrentXp: " + CurrentXp);
        }
        else
        {
            //Debug.LogError("Failed to load CurrentXp from save file.");
        }

        if (float.TryParse(sr.ReadLine(), out float loadedTargetXp))
        {
            TargetXp = loadedTargetXp;
            //Debug.Log("Loaded TargetXp: " + TargetXp);
        }
        else
        {
            //Debug.LogError("Failed to load TargetXp from save file.");
        }

        isDirty = true;
    }*/
    
    // Using PlayerPrefs to save and load the level progress
    public void SaveLevelProgress()
    {
        PlayerPrefs.SetInt("Level", Level);
        PlayerPrefs.SetFloat("CurrentXp", CurrentXp);
        PlayerPrefs.SetFloat("TargetXp", TargetXp);
        PlayerPrefs.Save();
        //Debug.Log(Level);
    }

    private void LoadLevelProgress()
    {
        if (!PlayerPrefs.HasKey("Level"))
        {
            Level = 0;
            CurrentXp = 0;
            TargetXp = XP_INCREMENT;
            //Debug.Log("Level is " + Level);

            return;
        }

        Level = PlayerPrefs.GetInt("Level", 0);
        CurrentXp = PlayerPrefs.GetFloat("CurrentXp", 0);
        TargetXp = PlayerPrefs.GetFloat("TargetXp", XP_INCREMENT);

        GameEventsManagerSO.instance.playerEvents.PlayerLevelChange(Level);
        isDirty = true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ExperienceController(int experience = 0)
    {
        CurrentXp += experience;
        isDirty = true;

        HandleLevelUp();
        HandleLevelDown();

        if (Level == 1 && CurrentXp < 0)
        {
            CurrentXp = 0;
        }
        if (Level == 20 && CurrentXp > TargetXp)
        {
            CurrentXp = TargetXp;
        }

        GameEventsManagerSO.instance.playerEvents.PlayerExperienceChange(CurrentXp);
    }

    private void HandleLevelUp()
    {
        while (CurrentXp >= TargetXp && Level < 20) 
        {
            CurrentXp -= TargetXp;
            Level++;
            if (Level < 20)
            {
                TargetXp += XP_INCREMENT;
            }
            else
            {
                CurrentXp = Mathf.Min(CurrentXp, TargetXp); // Prevent excess XP when max level is reached
            }
            isDirty = true;

            GameEventsManagerSO.instance.playerEvents.PlayerLevelChange(Level);
        }
    }

    private void HandleLevelDown()
    {
        while (CurrentXp < 0 && Level > 1)
        {
            Level--;
            TargetXp -= XP_INCREMENT;
            CurrentXp += TargetXp;

            GameEventsManagerSO.instance.playerEvents.PlayerLevelChange(Level);
            isDirty = true;
        }
    }

    private void UpdateUI()
    {
        LevelText.text = $"Player Level: {Level}";
        ExperienceText.text = $"{CurrentXp} / {TargetXp} EXP";
        XpProgressBar.fillAmount = CurrentXp / TargetXp;
    }
    
    // Method to reset the player's progress
    public void ResetProgressLevel()
    {
        Level = 0;
        CurrentXp = 0;
        TargetXp = XP_INCREMENT;
        isDirty = true;
        SaveLevelProgress();
    }
}
