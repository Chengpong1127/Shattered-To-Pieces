using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ConnectTests
{


    [Test]
    public void ConnectSingleComponentTest()
    {
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var c1 = componentFactory.CreateGameComponentObject(0);
        var c2 = componentFactory.CreateGameComponentObject(0);
        var info = new ConnectionInfo{ linkedTargetID = 1, connectorRotation = 0f };
        c2.ConnectToParent(c1, info);

        var c2_info = c2.DumpInfo();
        Assert.AreEqual(c2_info.connectorInfo.linkedTargetID, 1);
        Assert.AreEqual(c2_info.connectorInfo.connectorRotation, 0f);
    }
}
