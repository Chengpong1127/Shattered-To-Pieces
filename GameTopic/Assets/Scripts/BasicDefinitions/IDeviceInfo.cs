using System;
using System.Collections.Generic;
public interface IDeviceInfo
{
    Guid DeviceID { get; set; }
    public Dictionary<int, int> GameComponentIDMap { get; }
    public Dictionary<int, ConnectorInfo> ConnecterMap { get; }
    string Encode();
    void Decode(string json);
}
