using System;
public interface IDeviceInfo
{
    Guid DeviceID { get; set; }
    string Encode();
    void Decode(string json);
}
public struct ConnecterPoint: IEquatable<ConnecterPoint>
{
    public int ComponentID;
    public int PointID;

    public bool Equals(ConnecterPoint other)
    {
        return ComponentID == other.ComponentID && PointID == other.PointID;
    }
}
