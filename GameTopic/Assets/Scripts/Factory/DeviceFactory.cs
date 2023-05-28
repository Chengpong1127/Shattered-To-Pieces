using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceFactory : MonoBehaviour
{
    public GameObject CreateDevice(IDeviceInfo info)
    {
        var device = new GameObject();
        var deviceComponent = device.AddComponent<Device>();
        deviceComponent.LoadDevice(info);
        return device;
    }
}
