using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ConnectTests
{

    [Test]
    public void SetComponentIDTest()
    {
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var component = componentFactory.CreateGameComponentObject(0);
        component.LocalComponentID = 5;

        Assert.AreEqual(component.LocalComponentID, 5);
        Assert.AreEqual(component.Connector.connectorID, 5);
    }


    [Test]
    public void ConnectSingleComponentTest()
    {
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var c1 = componentFactory.CreateGameComponentObject(0);
        var c2 = componentFactory.CreateGameComponentObject(0);
        c1.LocalComponentID = 0;
        c2.LocalComponentID = 1;
        var info = new ConnectionInfo{ linkedConnectorID = 0, linkedTargetID = 1, connectorRotation = 0f };
        c2.Connect(c1, info);

        Assert.AreEqual(c1.Connector.connectorID, 0);
        Assert.AreEqual(c2.Connector.connectorID, 1);
        var c2_info = c2.DumpInfo();
        Assert.AreEqual(c2_info.connectorInfo.linkedConnectorID, 0);
        Assert.AreEqual(c2_info.connectorInfo.linkedTargetID, 1);
        Assert.AreEqual(c2_info.connectorInfo.connectorRotation, 0f);
    }
}
