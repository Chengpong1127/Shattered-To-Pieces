using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AbilityManagerTest
{
    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public void CreateAbilityManagerTest(int entryCount)
    {
        var device = new GameObject().AddComponent<Device>();
        var abilityManager = new AbilityManager(device, entryCount);
        Assert.AreEqual(abilityManager.AbilityInputEntries.Count, entryCount);
    }

    [Test]
    public void SetAbilityTest()
    {
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        var ability = new Ability("test");
        var ability2 = new Ability("test2");
        var ability3 = new Ability("test3");
        var ability4 = new Ability("test4");
        abilityManager.SetAbilityToEntry(0, ability);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], ability);

        abilityManager.SetAbilityToEntry(0, ability2);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], ability2);

        abilityManager.SetAbilityToEntry(0, ability3);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], ability3);

        abilityManager.SetAbilityToEntry(0, ability4);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], ability4);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[1], ability3);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[2], ability2);

    }

    [Test]
    public void SetPathTest(){
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        var path = "test";
        abilityManager.SetPath(0, path);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].InputPath, path);
    }

    [Test]
    public void SetAbilityOutOfEntryTest(){
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        var ability = new Ability("test");
        var ability2 = new Ability("test2");
        abilityManager.SetAbilityOutOfEntry(ability);
        var abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 4);

        abilityManager.SetAbilityOutOfEntry(ability2);
        abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 5);
    }

    [Test]
    public void LoadDeviceTest(){
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        
        var abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 3);
        abilityManager.SetAbilityToEntry(0, abilityListOutOfEntry[0]);
        abilityManager.SetAbilityToEntry(1, abilityListOutOfEntry[1]);

        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], abilityListOutOfEntry[0]);
        Assert.AreEqual(abilityManager.AbilityInputEntries[1].Abilities[0], abilityListOutOfEntry[1]);

        abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 1);

        abilityManager.ReloadDeviceAbilities();

        abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 3);
    }

    class TestDevice : IDevice
    {
        public IGameComponent RootGameComponent { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public IInfo Dump()
        {
            throw new System.NotImplementedException();
        }

        public List<Ability> getAbilityList()
        {
            List<Ability> abilityList = new List<Ability>();
            abilityList.Add(new Ability("test"));
            abilityList.Add(new Ability("test2"));
            abilityList.Add(new Ability("test3"));

            return abilityList;
        }

        public void Load(IInfo info)
        {
            throw new System.NotImplementedException();
        }
    }
}
