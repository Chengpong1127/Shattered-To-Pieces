using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ConnectTests
{

    
    [Test]
    [TestCase("Square")]
    public void CreateSingleComponentTest(string componentName)
    {
        var componentFactory = new GameObject().AddComponent<GameComponentFactory>();
        var c = componentFactory.CreateGameComponentObject(componentName);
        var info = c.Dump() as GameComponentInfo;
        Assert.AreEqual(componentName, info.ComponentName);
        Assert.AreNotEqual(c.Connector, null);
        Assert.AreEqual(info.ConnectionInfo, ConnectionInfo.NoConnection());
        Assert.AreEqual(c.GetParent(), null);
        Assert.AreEqual(c.GetChildren().Count, 0);
        Assert.True(c.Connector != null);

    }


    [Test]
    [TestCase("ControlRoom", 0)]
    [TestCase("ControlRoom", 1)]
    public void SingleConnectionTest(string componentName, int targetID)
    {
        var componentFactory = new GameObject().AddComponent<GameComponentFactory>();
        var c1 = componentFactory.CreateGameComponentObject(componentName);
        var c2 = componentFactory.CreateGameComponentObject(componentName);
        var connectionInfo = new ConnectionInfo{
            linkedTargetID = targetID,
        };
        c1.ConnectToParent(c2, connectionInfo);
        var info = c1.Dump() as GameComponentInfo;
        Assert.AreEqual(targetID, info.ConnectionInfo.linkedTargetID);

        c1.DisconnectFromParent();
        info = c1.Dump() as GameComponentInfo;

        Assert.AreEqual(info.ConnectionInfo, ConnectionInfo.NoConnection());
        
    }
    [Test]
    public void GetParentTest(){
        var componentFactory = new GameObject().AddComponent<GameComponentFactory>();
        var c1 = componentFactory.CreateGameComponentObject("Square");
        var c2 = componentFactory.CreateGameComponentObject("Square");
        var c3 = componentFactory.CreateGameComponentObject("Square");
        var connectionInfo = new ConnectionInfo{
            linkedTargetID = 0,
        };
        c1.ConnectToParent(c2, connectionInfo);
        c2.ConnectToParent(c3, connectionInfo);
        Assert.True(c1.GetParent() != null);
        Assert.True(c1.GetParent() == c2);
        Assert.AreEqual(c2.GetParent(), c3);
        c1.DisconnectFromParent();
        Assert.True(c1.GetParent() == null);
    }


        [Test]
    public void GetChildrenTest(){
        var componentFactory = new GameObject().AddComponent<GameComponentFactory>();
        var c1 = componentFactory.CreateGameComponentObject("Square");
        var c2 = componentFactory.CreateGameComponentObject("Square");
        var c3 = componentFactory.CreateGameComponentObject("Square");
        var c4 = componentFactory.CreateGameComponentObject("Square");
        var connectionInfo = new ConnectionInfo{
            linkedTargetID = 0,
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

        c1.DisconnectFromParent();
        Assert.True(c1.GetChildren().Count == 0);
        c2.DisconnectFromParent();
        Assert.True(c2.GetChildren().Count == 0);
        c3.DisconnectFromParent();
        Assert.True(c3.GetChildren().Count == 0);
        c4.DisconnectFromParent();
        Assert.True(c4.GetChildren().Count == 0);

        c1.ConnectToParent(c2, connectionInfo);
        c3.ConnectToParent(c2, connectionInfo);
        c4.ConnectToParent(c2, connectionInfo);
        Assert.True(c1.GetChildren().Count == 0);
        Assert.True(c2.GetChildren().Count == 3);
        Assert.True(c3.GetChildren().Count == 0);
        Assert.True(c4.GetChildren().Count == 0);
        
        Assert.AreEqual(c2.GetChildren()[0], c1);
        Assert.AreEqual(c2.GetChildren()[1], c3);
        Assert.AreEqual(c2.GetChildren()[2], c4);

        Assert.AreEqual(c1.GetParent(), c2);
        Assert.AreEqual(c3.GetParent(), c2);
        Assert.AreEqual(c4.GetParent(), c2);
    }
    [Test]
    public void GetRootTest(){
        var componentFactory = new GameObject().AddComponent<GameComponentFactory>();
        var c1 = componentFactory.CreateGameComponentObject("Square");
        var c2 = componentFactory.CreateGameComponentObject("Square");
        var c3 = componentFactory.CreateGameComponentObject("Square");
        var c4 = componentFactory.CreateGameComponentObject("Square");
        var connectionInfo = new ConnectionInfo{
            linkedTargetID = 0,
        };
        c1.ConnectToParent(c2, connectionInfo);
        c2.ConnectToParent(c3, connectionInfo);
        Assert.True(c1.GetRoot() == c3);
        Assert.True(c2.GetRoot() == c3);
        Assert.True(c3.GetRoot() == c3);
        Assert.True(c4.GetRoot() == c4);
        c1.DisconnectFromParent();
        Assert.True(c1.GetRoot() == c1);
        Assert.True(c2.GetRoot() == c3);
        Assert.True(c3.GetRoot() == c3);
        Assert.True(c4.GetRoot() == c4);
        c2.DisconnectFromParent();
        Assert.True(c1.GetRoot() == c1);
        Assert.True(c2.GetRoot() == c2);
        Assert.True(c3.GetRoot() == c3);
        Assert.True(c4.GetRoot() == c4);
        c3.DisconnectFromParent();
        Assert.True(c1.GetRoot() == c1);
        Assert.True(c2.GetRoot() == c2);
        Assert.True(c3.GetRoot() == c3);
        Assert.True(c4.GetRoot() == c4);
        c4.DisconnectFromParent();
        Assert.True(c1.GetRoot() == c1);
        Assert.True(c2.GetRoot() == c2);
        Assert.True(c3.GetRoot() == c3);
        Assert.True(c4.GetRoot() == c4);
    }

}
