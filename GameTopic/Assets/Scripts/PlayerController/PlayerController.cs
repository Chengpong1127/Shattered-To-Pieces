using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class PlayerController : IPlayerController
{

    private DeviceInfo deviceInfo;
    private Dictionary<String, int> ComponentCounts;
    private string playerId="1";//這邊先預設id是1
    public PlayerController()
    {
        deviceInfo = new DeviceInfo();
        ComponentCounts = new Dictionary<String, int>();
        CreateDeviceInstance();
    }

    public void SaveComponentCount(Dictionary<String, int> counts)
    {
        ComponentCounts = counts;

        var saveData = new SaveData(playerId, ComponentCounts);


        string jsonData = JsonConvert.SerializeObject(saveData);


        string fileName = "DefaultConfig.json";
        string filePath = Path.Combine(Application.dataPath, "Scripts", fileName);


        File.WriteAllText(filePath, jsonData);
        Debug.Log("Saved player data to: " + filePath);
    }
    public void CreateDeviceInstance()
    {
        string jsonFileName = "DeviceInfo.json";
        string jsonPath = Path.Combine(Application.dataPath, "Scripts", jsonFileName);
        string json = File.ReadAllText(jsonPath);
        deviceInfo.Decode(json);
        //foreach (var component in deviceInfo.ComponentList)
        //{
        //    Debug.Log("Component ID: " + component.GameComponentID);
        //}
    }

    public Dictionary<String, int> LoadComponentCount()
    {
        string fileName = "DefaultConfig.json";
        string filePath = Path.Combine(Application.dataPath, "Scripts", fileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            var saveData = JsonConvert.DeserializeObject<SaveData>(json); ;

            if (saveData.ComponentCounts != null)
            {
                ComponentCounts = saveData.ComponentCounts;
                return saveData.ComponentCounts;
            }
            else
            {
                Debug.Log("no component");
                return null;
            }
            
        }
        return null;
    }


   
}

