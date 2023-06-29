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
        //print component info
        foreach (var componentInfo in GameComponentInfoMap){
            Debug.Log("componentGUID: " + componentInfo.Value.componentGUID + " ComponentID: "+ componentInfo.Key + "ConnectorInfo: " + " " + componentInfo.Value.connectorInfo.linkedTargetID);

        }

    }
}

public struct GameComponentInfo{
    public int componentGUID;
    public ConnectionInfo connectorInfo;
    

}

// dump or load info for IConnector
public struct ConnectionInfo
{
    public int linkedTargetID;
    public float connectorRotation;
    public bool IsConnected => linkedTargetID != -1;
    public static ConnectionInfo NoConnection(){
        return new ConnectionInfo{
            linkedTargetID = -1,
            connectorRotation = 0f
        };
    }
}