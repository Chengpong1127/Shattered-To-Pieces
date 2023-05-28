using Newtonsoft.Json;
using System;
using System.Collections.Generic;
public class DeviceInfo : IDeviceInfo
{
    private Guid deviceID;
    private Dictionary<int, int> _componentIDMap; // <DeviceComponentID, GameComponentID>


    private Dictionary<int, int> ComponentIDMap;
    private Dictionary<int, List<ConnectorPoint>> _connecterMap;

    Guid IDeviceInfo.DeviceID { get => deviceID; set => deviceID = value; }

    public Dictionary<int, int> GameComponentIDMap{ get => _componentIDMap; set => _componentIDMap = value; }

    public Dictionary<int, List<ConnectorPoint>> ConnecterMap { get => _connecterMap; set => _connecterMap = value; }
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
