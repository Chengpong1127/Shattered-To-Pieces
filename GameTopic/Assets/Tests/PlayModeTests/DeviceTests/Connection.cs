using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Connection
{
    [Test]
    public void CreateDeviceTest(){
        var componentFactory = new GameObject().AddComponent<DeviceFactory>();
        var device = new GameObject().AddComponent<Device>();
        device.GameComponentFactory = componentFactory;
        var c1 = componentFactory.CreateGameComponentObject(0);

        device.RootGameComponent = c1;
        var info = device.Dump() as DeviceInfo;
        Assert.True(info != null);
        
    }
}
