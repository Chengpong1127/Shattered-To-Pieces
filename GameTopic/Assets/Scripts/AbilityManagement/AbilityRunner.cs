using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using AbilitySystem;
using AbilitySystem.Authoring;
using System.Linq;

public class AbilityRunner: MonoBehaviour{
    public AbilityManager AbilityManager { get; private set; }
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
        }
    }
    public void StartAbility(int entryIndex){
        ActivateEntry(AbilityManager.AbilityInputEntries[entryIndex].Abilities);
    }
    public void StartAbility(string entryKey){
        for (int i = 0; i < AbilityManager.AbilityInputEntries.Count; i++)
        {
            if(AbilityManager.AbilityInputEntries[i].InputPath == entryKey){
                StartAbility(i);
            }
        }
    }

    private void ActivateEntry(List<GameComponentAbility> abilities){
        foreach (var ability in abilities){
            StartCoroutine(ability.AbilitySpec.TryActivateAbility());
        }
    }

}