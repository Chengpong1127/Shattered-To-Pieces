using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AbilityRunnerTest
{
    int testNumber = 0;
    [Test]
    public void ConstructAbilityRunnerTest()
    {
        testNumber = 0;
        var abilityRunner = new GameObject().AddComponent<AbilityRunner>();
        Assert.True(abilityRunner != null);
        abilityRunner.AbilityManager = GetTestAbilityManager();

        abilityRunner.StartAbility(0);
        Assert.AreEqual(testNumber, 1);
        abilityRunner.StartAbility(1);
        Assert.AreEqual(testNumber, 2);
        abilityRunner.EndAbility(0);
        abilityRunner.EndAbility(1);

    }

    [UnityTest]
    public IEnumerator AbilityRunnerTestRunning()
    {
        testNumber = 0;
        var abilityRunner = new GameObject().AddComponent<AbilityRunner>();
        Assert.True(abilityRunner != null);
        abilityRunner.AbilityManager = GetTestAbilityManager();

        abilityRunner.StartAbility(2);
        Assert.AreEqual(testNumber, 3);
        for (int i = 0; i < 10; i++)
        {
            yield return null;
            Assert.AreEqual(testNumber, 3 + i + 1);
        }
        abilityRunner.EndAbility(2);
        Assert.AreEqual(testNumber, 5);
        yield return null;
    }


    public AbilityManager GetTestAbilityManager(){
        
        var abilityManager = new AbilityManager(new Device(), 3);
        var ability1 = new Ability("test1", ()=>{this.testNumber = 1;});
        var ability2 = new Ability("test2", ()=>{this.testNumber = 2;});

        abilityManager.SetAbilityToEntry(0, ability1);
        abilityManager.SetAbilityToEntry(1, ability2);

        var ability3 = new Ability("test3", ()=>{this.testNumber = 3;}, () => {this.testNumber++;}, () => {this.testNumber = 5;}, null);
        abilityManager.SetAbilityToEntry(2, ability3);
        return abilityManager;

    }
}
