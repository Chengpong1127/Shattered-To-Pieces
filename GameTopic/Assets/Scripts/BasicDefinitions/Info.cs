using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public class DeviceInfo
{

    public Dictionary<int, GameComponentInfo> GameComponentInfoMap = new Dictionary<int, GameComponentInfo>();
    public void Decode(string json)
    {
        var info = JsonConvert.DeserializeObject<DeviceInfo>(json);
    }

    public string Encode()
    {
        var result = JsonConvert.SerializeObject(this);
        return result;
    }
    public void printAllInfo(){
        if (GameComponentInfoMap == null){
            Debug.Log("GameComponentInfoMap is null");
            return;
        }
        if (GameComponentInfoMap.Count == 0){
            Debug.Log("GameComponentInfoMap is empty");
        }
        //print component number
        Debug.Log("GameComponentInfoMap.Count: " + GameComponentInfoMap.Count);

    }
}

public struct GameComponentInfo{
    public int componentGUID;
    public ConnectorInfo connectorInfo;
    

}