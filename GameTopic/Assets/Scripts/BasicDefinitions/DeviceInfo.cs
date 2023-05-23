using System;
public interface IDeviceInfo
{
    Guid DeviceID { get; set; }
    string Encode();
    void Decode(string json);
}
