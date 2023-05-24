using Newtonsoft.Json;
using System;
using System.Collections.Generic;
public class DeviceInfo : IDeviceInfo
{
    private Guid deviceID;
    public List<IGameComponent> ComponentList;


    private Dictionary<int, int> ComponentIDMap;
    private Dictionary<ConnecterPoint, ConnecterPoint> ConnecterMap;

    Guid IDeviceInfo.DeviceID { get => deviceID; set => deviceID = value; }


    public void Decode(string json)
    {
        var info = JsonConvert.DeserializeObject<DeviceInfo>(json);
        deviceID = info.deviceID;
        ComponentList = info.ComponentList;
    }

    public string Encode()
    {
        var result = JsonConvert.SerializeObject(this);
        return result;
    }
}
