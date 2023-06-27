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

        Assert.AreEqual(device.ComponentMap[0].Connector.Dump(), ConnectorInfo.NoConnection(0));

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
