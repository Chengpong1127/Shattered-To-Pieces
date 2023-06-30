using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Connection
{
    [Test]
    public void CreateDeviceConnectionTest(){
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var device = createSimpleDevice();
        device.GameComponentFactory = componentFactory;
        var deviceInfo = createSimpleDeviceInfo();
        //device.LoadDevice(deviceInfo);
        
    }

    [Test]
    public void SetConnectionTest(){
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var device = createSimpleDevice();
        device.GameComponentFactory = componentFactory;
        var deviceInfo = createSimpleDeviceInfo();
        //device.LoadDevice(deviceInfo);
    }

    private Device createSimpleDevice(){
        var device = new GameObject("Device").AddComponent<Device>();
        return device;
    }

    private DeviceInfo createSimpleDeviceInfo(){
        var deviceInfo = new DeviceInfo();
        
        return deviceInfo;
    }
}
