using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public class DeviceInfo: IInfo
{

    public Dictionary<int, GameComponentInfo> gameComponentInfoMap;
    
    public void Decode(string json)
    {
        var info = JsonConvert.DeserializeObject<DeviceInfo>(json);
    }

    public string Encode()
    {
        var result = JsonConvert.SerializeObject(this);
        return result;
    }
}

public struct GameComponentInfo: IInfo{
    public int componentGUID;
    public ConnectionInfo connectionInfo;
    

}

// dump or load info for IConnector
public struct ConnectionInfo: IInfo
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

public interface IDumpable{
    public IInfo Dump();
}

public class TreeInfo: IInfo{
    public int rootID;
    public Dictionary<int, object> NodeInfoMap = new Dictionary<int, object>();
    public List<(int, int)> EdgeInfoList = new List<(int, int)>();
}

public interface IInfo{

}