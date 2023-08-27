using System;
using UnityEngine;
using System.Collections.Generic;

public class AbilityRunner: MonoBehaviour{
    public AbilityManager AbilityManager { get; private set; }
    public static AbilityRunner CreateInstance(GameObject where, AbilityManager abilityManager){
        if (where == null){
            throw new ArgumentNullException(nameof(where));
        }
        var abilityRunner = where.AddComponent<AbilityRunner>();
        abilityRunner.AbilityManager = abilityManager ?? throw new ArgumentNullException(nameof(abilityManager));
        return abilityRunner;
    }
    public void StartAbility(int entryIndex){
        ActivateEntry(AbilityManager.AbilityInputEntries[entryIndex].Abilities);
    }
    public void CancelAbility(int entryIndex){
        CancelEntry(AbilityManager.AbilityInputEntries[entryIndex].Abilities);
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
    private void CancelEntry(List<GameComponentAbility> abilities){
        foreach (var ability in abilities){
            ability.AbilitySpec.CancelAbility();
        }
    }

}