using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AbilityRunner: MonoBehaviour{
    public AbilityManager AbilityManager { get; private set; }
    private readonly HashSet<int> RunningAbilitySet = new();

    public static AbilityRunner CreateInstance(GameObject where, AbilityManager abilityManager){
        if (where == null){
            throw new ArgumentNullException(nameof(where));
        }
        var abilityRunner = where.AddComponent<AbilityRunner>();
        abilityRunner.AbilityManager = abilityManager ?? throw new System.ArgumentNullException(nameof(abilityManager));
        return abilityRunner;
    }
    public void BindInputActionsToRunner(InputAction[] abilityActions){
        if (abilityActions == null){
            throw new ArgumentNullException(nameof(abilityActions));
        }
        if (abilityActions.Length != AbilityManager.AbilityInputEntryNumber){
            throw new ArgumentException("The length of abilityActions should be the same as the length of abilityInputEntries");
        }
        for (int i = 0; i < abilityActions.Length; i++)
        {
            var abilityNumber = i;
            abilityActions[abilityNumber].AddBinding(AbilityManager.AbilityInputEntries[abilityNumber].InputPath);
            abilityActions[abilityNumber].started += ctx => StartAbility(abilityNumber);
            abilityActions[abilityNumber].canceled += ctx => EndAbility(abilityNumber);
        }
    }
    public void StartAbility(int entryIndex){
        AbilityManager.AbilityInputEntries[entryIndex].StartAllAbilities();
        RunningAbilitySet.Add(entryIndex);
    }
    public void StartAbility(string entryKey){
        for (int i = 0; i < AbilityManager.AbilityInputEntries.Count; i++)
        {
            if(AbilityManager.AbilityInputEntries[i].InputPath == entryKey){
                StartAbility(i);
            }
        }
    }

    public void EndAbility(int entryIndex){
        if (!RunningAbilitySet.Contains(entryIndex)){
            throw new ArgumentException("The ability is not running");
        }
        AbilityManager.AbilityInputEntries[entryIndex].EndAllAbilities();
        RunningAbilitySet.Remove(entryIndex);
    }

    public void EndAbility(string entryKey){
        for (int i = 0; i < AbilityManager.AbilityInputEntries.Count; i++)
        {
            if(AbilityManager.AbilityInputEntries[i].InputPath == entryKey){
                EndAbility(i);
            }
        }
    }

    private void Update() {
        foreach (var entryIndex in RunningAbilitySet){
            AbilityManager.AbilityInputEntries[entryIndex].RunAllAbilitiesForEachFrame();
        }
    }

}