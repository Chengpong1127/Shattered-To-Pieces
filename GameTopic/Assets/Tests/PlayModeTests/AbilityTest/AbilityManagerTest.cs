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
        var device = new TestDevice();
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
    public void SetAbilityTest2()
    {
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        var abilityOutofentry = abilityManager.GetAbilitiesOutOfEntry();

        abilityManager.SetAbilityToEntry(0, abilityOutofentry[0]);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], abilityOutofentry[0]);

        abilityManager.SetAbilityToEntry(0, abilityOutofentry[1]);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], abilityOutofentry[1]);

        abilityManager.SetAbilityToEntry(0, abilityOutofentry[2]);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], abilityOutofentry[2]);

        abilityManager.SetAbilityToEntry(0, abilityOutofentry[3]);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], abilityOutofentry[3]);

        var removed = abilityOutofentry[0];

        abilityOutofentry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityOutofentry[0], removed);
    }

    [Test]
    public void SetPathTest(){
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        var path = "test";
        abilityManager.SetBinding(0, path);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].InputPath, path);

        path = "test2";
        abilityManager.SetBinding(0, path);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].InputPath, path);
    }

    [Test]
    public void SetAbilityOutOfEntryTest(){
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        var abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();

        abilityManager.SetAbilityToEntry(0, abilityListOutOfEntry[0]);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], abilityListOutOfEntry[0]);

        abilityManager.SetAbilityToEntry(0, abilityListOutOfEntry[1]);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], abilityListOutOfEntry[1]);
        Assert.AreEqual(abilityManager.GetAbilitiesOutOfEntry().Count, 2);

        abilityManager.SetAbilityOutOfEntry(abilityListOutOfEntry[0]);
        Assert.AreEqual(abilityManager.GetAbilitiesOutOfEntry().Count, 3);

        abilityManager.SetAbilityOutOfEntry(abilityListOutOfEntry[1]);
        Assert.AreEqual(abilityManager.GetAbilitiesOutOfEntry().Count, 4);

        abilityManager.SetAbilityOutOfEntry(abilityListOutOfEntry[2]);
        Assert.AreEqual(abilityManager.GetAbilitiesOutOfEntry().Count, 4);

    }
    [Test]
    public void AbilityRemoveTest(){
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        var outOfEntryAbility = abilityManager.GetAbilitiesOutOfEntry();
        var ability = outOfEntryAbility[0];

        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities.Count, 0);
        abilityManager.SetAbilityToEntry(0, ability);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], ability);
        abilityManager.SetAbilityToEntry(1, ability);
        Assert.AreEqual(abilityManager.AbilityInputEntries[1].Abilities[0], ability);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities.Count, 0);

        abilityManager.SetAbilityOutOfEntry(ability);
        Assert.AreEqual(abilityManager.AbilityInputEntries[1].Abilities.Count, 0);
    }

    [Test]
    public void LoadDeviceTest(){
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        
        var abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 4);
        abilityManager.SetAbilityToEntry(0, abilityListOutOfEntry[0]);
        abilityManager.SetAbilityToEntry(1, abilityListOutOfEntry[1]);

        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], abilityListOutOfEntry[0]);
        Assert.AreEqual(abilityManager.AbilityInputEntries[1].Abilities[0], abilityListOutOfEntry[1]);

        abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 2);

        abilityManager.ReloadDeviceAbilities();

        abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 4);
    }

    [Test]
    public void UpdateDeviceAbilitiesTest(){
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        
        var abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 4);
        abilityManager.SetAbilityToEntry(0, abilityListOutOfEntry[0]);
        abilityManager.SetAbilityToEntry(1, abilityListOutOfEntry[1]);

        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities[0], abilityListOutOfEntry[0]);
        Assert.AreEqual(abilityManager.AbilityInputEntries[1].Abilities[0], abilityListOutOfEntry[1]);

        abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 2);

        abilityManager.UpdateDeviceAbilities();

        abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 2);
    }
    [Test]
    public void ReloadDeviceAbilitiesTest(){
        var device = new TestDevice();
        var abilityManager = new AbilityManager(device);
        
        var abilityListOutOfEntry = abilityManager.GetAbilitiesOutOfEntry();
        Assert.AreEqual(abilityListOutOfEntry.Count, 4);
        abilityManager.SetAbilityToEntry(0, abilityListOutOfEntry[0]);
        abilityManager.SetAbilityToEntry(1, abilityListOutOfEntry[1]);
        Assert.AreEqual(abilityManager.GetAbilitiesOutOfEntry().Count, 2);

        abilityManager.ReloadDeviceAbilities();
        Assert.AreEqual(abilityManager.GetAbilitiesOutOfEntry().Count, 4);
        Assert.AreEqual(abilityManager.AbilityInputEntries[0].Abilities.Count, 0);

        abilityManager.ReloadDeviceAbilities();
        Assert.AreEqual(abilityManager.GetAbilitiesOutOfEntry().Count, 4);
    }

    class TestDevice : IDevice
    {
        public IGameComponent RootGameComponent { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public IInfo Dump()
        {
            throw new System.NotImplementedException();
        }

        List<Ability> abilityList = new List<Ability>();
        public TestDevice(){
            abilityList.Add(new Ability("test"));
            abilityList.Add(new Ability("test2"));
            abilityList.Add(new Ability("test3"));
            abilityList.Add(new Ability("test4"));
        }
        public void AddAbility(Ability ability)
        {
            abilityList.Add(ability);
        }

        public List<Ability> GetAbilityList()
        {
            return abilityList;
        }

        public void Load(IInfo info)
        {
            throw new System.NotImplementedException();
        }
    }
}
