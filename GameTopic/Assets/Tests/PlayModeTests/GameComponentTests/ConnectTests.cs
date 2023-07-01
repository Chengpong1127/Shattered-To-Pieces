using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ConnectTests
{

    
    [Test]
    [TestCase(0)]
    public void CreateSingleComponentTest(int componentGUID)
    {
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var c = componentFactory.CreateGameComponentObject(componentGUID);
        var info = c.Dump() as GameComponentInfo;
        Assert.AreEqual(componentGUID, info.componentGUID);
        Assert.AreNotEqual(c.Connector, null);
        Assert.AreEqual(info.connectionInfo, ConnectionInfo.NoConnection());
        Assert.AreEqual(c.GetParent(), null);
        Assert.AreEqual(c.GetChildren().Count, 0);
        Assert.True(c.Connector != null);

    }


    [Test]
    [TestCase(0, 0, 0f)]
    [TestCase(0, 1, 0f)]
    [TestCase(0, 2, 0f)]
    [TestCase(0, 3, 0f)]
    [TestCase(0, 0, 90f)]
    public void SingleConnectionTest(int componentGUID, int targetID, float connectorRotation)
    {
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var c1 = componentFactory.CreateGameComponentObject(0);
        var c2 = componentFactory.CreateGameComponentObject(0);
        var connectionInfo = new ConnectionInfo{
            linkedTargetID = targetID,
            connectorRotation = connectorRotation
        };
        c1.ConnectToParent(c2, connectionInfo);
        var info = c1.Dump() as GameComponentInfo;
        Assert.AreEqual(targetID, info.connectionInfo.linkedTargetID);
        Assert.AreEqual(connectorRotation, info.connectionInfo.connectorRotation);

        c1.DisconnectFromParent();
        info = c1.Dump() as GameComponentInfo;

        Assert.AreEqual(info.connectionInfo, ConnectionInfo.NoConnection());
        
    }
    [Test]
    public void GetParentTest(){
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var c1 = componentFactory.CreateGameComponentObject(0);
        var c2 = componentFactory.CreateGameComponentObject(0);
        var c3 = componentFactory.CreateGameComponentObject(0);
        var connectionInfo = new ConnectionInfo{
            linkedTargetID = 0,
            connectorRotation = 0f
        };
        c1.ConnectToParent(c2, connectionInfo);
        c2.ConnectToParent(c3, connectionInfo);
        Assert.True(c1.GetParent() == c2);
        Assert.AreEqual(c2.GetParent(), c3);
        c1.DisconnectFromParent();
        Assert.True(c1.GetParent() == null);
    }
    [Test]
    public void GetChildrenTest(){
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var c1 = componentFactory.CreateGameComponentObject(0);
        var c2 = componentFactory.CreateGameComponentObject(0);
        var c3 = componentFactory.CreateGameComponentObject(0);
        var connectionInfo = new ConnectionInfo{
            linkedTargetID = 0,
            connectorRotation = 0f
        };
        c1.ConnectToParent(c2, connectionInfo);
        c2.ConnectToParent(c3, connectionInfo);
        Assert.True(c1.GetChildren().Count == 0);
        Assert.True(c2.GetChildren().Count == 1);
        Assert.True(c3.GetChildren().Count == 1);
        Assert.True(c2.GetChildren()[0] == c1);
        Assert.True(c3.GetChildren()[0] == c2);
        c1.DisconnectFromParent();
        Assert.True(c1.GetChildren().Count == 0);
        Assert.True(c2.GetChildren().Count == 0);
        Assert.True(c3.GetChildren().Count == 1);
        Assert.True(c3.GetChildren()[0] == c2);
        
    }

}
