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
        device.LoadDevice(deviceInfo);
        var dumpInfo = device.DumpDevice();
        Assert.AreEqual(deviceInfo.GameComponentInfoMap.Count, 2);
        Assert.AreEqual(device.ComponentMap[0].ComponentGUID, 0);
        Assert.AreEqual(device.ComponentMap[1].ComponentGUID, 1);


        Assert.AreEqual(device.ComponentMap[0].Connector.Dump().IsConnected, false);

        Assert.AreEqual(device.ComponentMap[1].Connector.Dump(), new ConnectionInfo{
            linkedTargetID = 0});
        
    }

    [Test]
    public void SetConnectionTest(){
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var device = createSimpleDevice();
        device.GameComponentFactory = componentFactory;
        var deviceInfo = createSimpleDeviceInfo();
        device.LoadDevice(deviceInfo);
        var dumpInfo = device.DumpDevice();


        Assert.AreEqual(device.ComponentMap[1].Connector.Dump(), new ConnectionInfo{
            linkedTargetID = 0});
        

        Assert.AreEqual(device.ComponentMap[0].Connector.Dump(), new ConnectionInfo{
            linkedTargetID = 1});

        Assert.AreEqual(device.ComponentMap[1].Connector.Dump(), new ConnectionInfo{
            linkedTargetID = 0});
    }

    private Device createSimpleDevice(){
        var device = new GameObject("Device").AddComponent<Device>();
        return device;
    }

    private DeviceInfo createSimpleDeviceInfo(){
        var deviceInfo = new DeviceInfo();
        deviceInfo.GameComponentInfoMap.Add(
            0, new GameComponentInfo{
                componentGUID = 0, 
                connectionInfo = ConnectionInfo.NoConnection()});
        deviceInfo.GameComponentInfoMap.Add(
            1, new GameComponentInfo{
                componentGUID = 1, 
                connectionInfo = new ConnectionInfo{
                    linkedTargetID = 0}});
        
        return deviceInfo;
    }
}
