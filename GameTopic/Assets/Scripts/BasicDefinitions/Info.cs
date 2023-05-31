using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
}

public struct GameComponentInfo{
    public int componentGUID;
    public ConnectorInfo connectorInfo;
    

}