using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DeviceTest
{
    [Test]
    public void CreateDefaultDeviceTest(){
        var componentFactory = new GameObject().AddComponent<GameComponentFactory>();
        var device = new GameObject().AddComponent<Device>();
        device.GameComponentFactory = componentFactory;
        device.Load(ResourceManager.Instance.LoadDefaultDeviceInfo());
        
        Assert.True(device.RootGameComponent is not null);
        Assert.True(device.AbilityManager is not null);

    }
}
