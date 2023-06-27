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

        Assert.AreEqual(device.ComponentMap[0].LocalComponentID, 0);
        Assert.AreEqual(device.ComponentMap[1].LocalComponentID, 1);

        Assert.AreEqual(device.ComponentMap[0].Connector.connectorID, 0);
        Assert.AreEqual(device.ComponentMap[1].Connector.connectorID, 1);

        Assert.AreEqual(device.ComponentMap[0].Connector.Dump().IsConnected, false);

        Assert.AreEqual(device.ComponentMap[1].Connector.Dump(), new ConnectorInfo{
            connectorID = 1,
            linkedConnectorID = 0, 
            linkedTargetID = 0});
        
    }


    [Test]
    public void AddComponentTest(){
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var device = createSimpleDevice();
        device.GameComponentFactory = componentFactory;
        var deviceInfo = createSimpleDeviceInfo();
        device.LoadDevice(deviceInfo);
        var dumpInfo = device.DumpDevice();
        Assert.AreEqual(deviceInfo.GameComponentInfoMap.Count, 2);
        Assert.AreEqual(device.ComponentMap[0].ComponentGUID, 0);
        Assert.AreEqual(device.ComponentMap[1].ComponentGUID, 1);

        Assert.AreEqual(device.ComponentMap[0].LocalComponentID, 0);
        Assert.AreEqual(device.ComponentMap[1].LocalComponentID, 1);

        Assert.AreEqual(device.ComponentMap[0].Connector.connectorID, 0);
        Assert.AreEqual(device.ComponentMap[1].Connector.connectorID, 1);

        Assert.AreEqual(device.ComponentMap[0].Connector.Dump().IsConnected, false);

        Assert.AreEqual(device.ComponentMap[1].Connector.Dump(), new ConnectorInfo{
            connectorID = 1,
            linkedConnectorID = 0, 
            linkedTargetID = 0});

        var newComponent = componentFactory.CreateGameComponentObject(1);
        Assert.AreEqual(newComponent.ComponentGUID, 1);
        Assert.AreEqual(newComponent.LocalComponentID, null);
        device.AddComponent(newComponent);
        Assert.AreEqual(device.ComponentMap[2].ComponentGUID, 1);
        Assert.AreEqual(device.ComponentMap[2].LocalComponentID, 2);
        Assert.AreEqual(device.ComponentMap[2].Connector.connectorID, 2);
        Assert.AreEqual(device.ComponentMap[2].Connector.Dump().IsConnected, false);
        
        
    }

    [Test]
    public void SetConnectionTest(){
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var device = createSimpleDevice();
        device.GameComponentFactory = componentFactory;
        var deviceInfo = createSimpleDeviceInfo();
        device.LoadDevice(deviceInfo);
        var dumpInfo = device.DumpDevice();


        Assert.AreEqual(device.ComponentMap[1].Connector.Dump(), new ConnectorInfo{
            connectorID = 1,
            linkedConnectorID = 0, 
            linkedTargetID = 0});
        
        device.SetConnection(new ConnectorInfo{
            connectorID = 0,
            linkedConnectorID = 1, 
            linkedTargetID = 1});
        
        Assert.AreEqual(device.ComponentMap[0].Connector.Dump(), new ConnectorInfo{
            connectorID = 0,
            linkedConnectorID = 1, 
            linkedTargetID = 1});

        Assert.AreEqual(device.ComponentMap[1].Connector.Dump(), new ConnectorInfo{
            connectorID = 1,
            linkedConnectorID = 0, 
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
                connectorInfo = ConnectorInfo.NoConnection(0)});
        deviceInfo.GameComponentInfoMap.Add(
            1, new GameComponentInfo{
                componentGUID = 1, 
                connectorInfo = new ConnectorInfo{
                    connectorID = 1,
                    linkedConnectorID = 0, 
                    linkedTargetID = 0}});
        
        return deviceInfo;
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ConnectionWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
