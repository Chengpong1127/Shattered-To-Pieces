using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ResourceManagerTest
{
    [Test]
    [TestCase("Square")]
    public void LoadPrefabTest(string filename)
    {
        var prefab = ResourceManager.Instance.LoadPrefab(filename);
        Assert.True(prefab != null);
    }

    [Test]
    public void LoadAllGameComponentDataTest()
    {
        var dataList = ResourceManager.Instance.LoadAllGameComponentData();
        var allfiles = Resources.LoadAll("GameComponentData");
        Assert.AreEqual(dataList.Count, allfiles.Length);

    }
}
