using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AbilityRunnerTest
{
    [Test]
    public void ConstructAbilityRunnerTest()
    {
        var abilityRunner = new GameObject().AddComponent<AbilityRunner>();
        Assert.True(abilityRunner != null);
        abilityRunner.AbilityManager = getTestAbilityManager();

        abilityRunner.StartAbility(0);
        abilityRunner.StartAbility(1);
        abilityRunner.EndAbiliey(0);
        abilityRunner.EndAbiliey(1);

    }



    public AbilityManager getTestAbilityManager(){
        var abilityManager = new AbilityManager(new Device(), 3);
        var ability1 = new Ability("test1", ()=>{Debug.Log("test1");});
        var ability2 = new Ability("test2", ()=>{Debug.Log("test2");});

        abilityManager.SetAbilityToEntry(0, ability1);
        abilityManager.SetAbilityToEntry(1, ability2);

        return abilityManager;

    }
}
