using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Gameframe.SaveLoad;

public class ResourceManager
{
    public static ResourceManager Instance { get; } = new ResourceManager();
    public readonly string PrefabPath = "Prefabs";
    public readonly string GameComponentDataPath = "GameComponentData";
    public readonly string DefaultDeviceInfoPath = "DefaultDeviceInfo";
    private SaveLoadManager localDeviceStorageManager;
    private ResourceManager() { 
        localDeviceStorageManager = SaveLoadManager.Create("BaseDirectory", "SavedDevice", SerializationMethodType.JsonDotNet);
    }
    public GameObject LoadPrefab(string filename){
        var path = Path.Combine(PrefabPath, filename);
        var prefab = Resources.Load<GameObject>(path);
        if(prefab == null){
            Debug.LogWarning("Cannot find prefab: " + path);
        }
        return prefab;
    }

    public List<GameComponentData> LoadAllGameComponentData(){
        var data = Resources.LoadAll<GameComponentData>(GameComponentDataPath);
        var dataList = new List<GameComponentData>();
        foreach(var d in data){
            dataList.Add(d);
        }
        return dataList;
    }

    public DeviceInfo LoadDefaultDeviceInfo(){
        var text = (TextAsset)Resources.Load(DefaultDeviceInfoPath);
        var info = JsonConvert.DeserializeObject<DeviceInfo>(text.text);
        Debug.Assert(info != null);
        return info;
    }

    public DeviceInfo LoadLocalDeviceInfo(string name){
        string filename = name + ".json";
        var deviceInfo = localDeviceStorageManager.Load<DeviceInfo>(filename);
        return deviceInfo;
    }

    public void SaveLocalDeviceInfo(DeviceInfo info, string name){
        Debug.Assert(info != null);
        string filename = name + ".json";
        localDeviceStorageManager.Save(info, filename);
    }


}
