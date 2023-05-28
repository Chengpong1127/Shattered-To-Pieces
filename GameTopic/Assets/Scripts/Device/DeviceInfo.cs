using Newtonsoft.Json;
using System;
using System.Collections.Generic;
public class DeviceInfo : IDeviceInfo
{
    private Guid deviceID;
    public List<int> _componentIDList;


    private Dictionary<int, int> ComponentIDMap;
    private Dictionary<int, List<ConnectorPoint>> _connecterMap;

    Guid IDeviceInfo.DeviceID { get => deviceID; set => deviceID = value; }

    public List<int> GameComponentIDList => _componentIDList;

    public Dictionary<int, List<ConnectorPoint>> ConnecterMap => _connecterMap;
    public void Decode(string json)
    {
        var info = JsonConvert.DeserializeObject<DeviceInfo>(json);
        deviceID = info.deviceID;
    }

    public string Encode()
    {
        var result = JsonConvert.SerializeObject(this);
        return result;
    }
}
