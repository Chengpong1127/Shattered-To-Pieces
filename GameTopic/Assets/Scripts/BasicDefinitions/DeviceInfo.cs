using System;
using System.Collections.Generic;
public interface IDeviceInfo
{
    Guid DeviceID { get; set; }
    public List<int> GameComponentIDList { get; }
    public Dictionary<int, List<ConnectorPoint>> ConnecterMap { get; }
    string Encode();
    void Decode(string json);
}
public struct ConnectorPoint: IEquatable<ConnectorPoint>
{
    public int ComponentID;
    public int TargetID;

    public bool Equals(ConnectorPoint other)
    {
        return ComponentID == other.ComponentID && TargetID == other.TargetID;
    }
}
