using System;
using System.Collections.Generic;
public interface IDeviceInfo
{
    Guid DeviceID { get; set; }
    public Dictionary<int, int> GameComponentIDMap { get; }
    public Dictionary<int, ConnectorPoint> ConnecterMap { get; }
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
