using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    [System.Serializable]
    class SaveData
    {
        public float elapsedTime;
        public string saveDate;
    }

    public float elapsedTime; // This will store the elapsed time
    private string saveFilePath;

    void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");
    }

    void Update()
    {
        elapsedTime += Time.deltaTime; // Update the elapsed time
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.elapsedTime = elapsedTime;
        data.saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            elapsedTime = data.elapsedTime;
            Debug.Log("Game Loaded: Elapsed Time = " + elapsedTime + ", Save Date = " + data.saveDate);
        }
        else
        {
            Debug.Log("No save file found");
        }
    }
}