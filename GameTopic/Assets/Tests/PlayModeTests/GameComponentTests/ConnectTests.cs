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
        var component = componentFactory.CreateGameObject(0).GetComponent<IGameComponent>();
        component.LocalComponentID = 5;

        Assert.AreEqual(component.LocalComponentID, 5);
        Assert.AreEqual(component.Connector.connectorID, 5);
    }


    [Test]
    public void ConnectSingleComponentTest()
    {
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var c1 = componentFactory.CreateGameObject(0).GetComponent<IGameComponent>();
        var c2 = componentFactory.CreateGameObject(0).GetComponent<IGameComponent>();
        c1.LocalComponentID = 0;
        c2.LocalComponentID = 1;
        var info = new ConnectorInfo{ connectorID = 1, linkedConnectorID = 0, linkedTargetID = 1, connectorRotation = 0f };
        c2.Connect(c1, info);

        Assert.AreEqual(c1.Connector.connectorID, 0);
        Assert.AreEqual(c2.Connector.connectorID, 1);
        var c2_info = c2.DumpInfo();
        Assert.AreEqual(c2_info.connectorInfo.connectorID, 1);
        Assert.AreEqual(c2_info.connectorInfo.linkedConnectorID, 0);
        Assert.AreEqual(c2_info.connectorInfo.linkedTargetID, 1);
        Assert.AreEqual(c2_info.connectorInfo.connectorRotation, 0f);
    }
}
