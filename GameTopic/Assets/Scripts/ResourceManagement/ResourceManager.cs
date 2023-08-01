using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class ResourceManager
{
    public static readonly string PrefabPath = "Prefabs";
    public static readonly string GameComponentDataPath = "GameComponentData";
    public static readonly string DefaultDeviceInfoPath = "DefaultDeviceInfo";
    public static GameObject LoadPrefab(string filename){
        var path = Path.Combine(PrefabPath, filename);
        var prefab = Resources.Load<GameObject>(path);
        if(prefab == null){
            Debug.LogWarning("Cannot find prefab: " + path);
        }
        return prefab;
    }

    public static List<GameComponentData> LoadAllGameComponentData(){
        var data = Resources.LoadAll<GameComponentData>(GameComponentDataPath);
        var dataList = new List<GameComponentData>();
        foreach(var d in data){
            dataList.Add(d);
        }
        return dataList;
    }

    public static DeviceInfo LoadDefaultDeviceInfo(){
        var text = (TextAsset)Resources.Load(DefaultDeviceInfoPath);
        var info = JsonConvert.DeserializeObject<DeviceInfo>(text.text);
        Debug.Assert(info != null);
        return info;
    }


}
