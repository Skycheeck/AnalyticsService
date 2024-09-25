using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveManager
{
    private readonly string _saveFile = Path.Combine(Application.persistentDataPath, "save.json");

    public void SaveState(AnalyticsServiceState analyticsServiceState)
    {
        string jsonString = JsonConvert.SerializeObject(analyticsServiceState);
        File.WriteAllText(_saveFile, jsonString);
    }

    public AnalyticsServiceState LoadState()
    {
        if (!File.Exists(_saveFile)) return new AnalyticsServiceState(3f);
        
        string fileContents = File.ReadAllText(_saveFile);
        return JsonConvert.DeserializeObject<AnalyticsServiceState>(fileContents);
    }
}