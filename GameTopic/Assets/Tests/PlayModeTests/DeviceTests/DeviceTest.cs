using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DeviceTest
{
    [Test]
    public void CreateDefaultDeviceTest(){
        var componentFactory = new GameComponentFactory();
        var device = new Device(componentFactory);
        device.Load(ResourceManager.Instance.LoadDefaultDeviceInfo());
        
        Assert.True(device.RootGameComponent is not null);
        Assert.True(device.AbilityManager is not null);

    }
}
