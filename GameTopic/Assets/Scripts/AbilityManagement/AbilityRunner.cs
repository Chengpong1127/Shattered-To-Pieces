using System;
using UnityEngine;
using System.Collections.Generic;

public class AbilityRunner: MonoBehaviour, IAbilityRunner{
    public AbilityManager AbilityManager { get; private set; }
    public static AbilityRunner CreateInstance(GameObject where, AbilityManager abilityManager){
        if (where == null){
            throw new ArgumentNullException(nameof(where));
        }
        var abilityRunner = where.AddComponent<AbilityRunner>();
        abilityRunner.AbilityManager = abilityManager ?? throw new ArgumentNullException(nameof(abilityManager));
        return abilityRunner;
    }
    public void StartSingleAbility(string abilityName){
        foreach (var ability in AbilityManager){
            if (ability.AbilityName == abilityName){
                ability.AbilitySpec.Runner = this;
                StartCoroutine(ability.AbilitySpec.TryActivateAbility());
                return;
            }
        }
    }
    public void StartEntryAbility(int entryIndex){
        ActivateEntry(AbilityManager.AbilityInputEntries[entryIndex].Abilities);
    }
    public void CancelSingleAbility(string abilityName){
        foreach (var ability in AbilityManager){
            if (ability.AbilityName == abilityName){
                ability.AbilitySpec.CancelAbility();
                return;
            }
        }
    }
    public void CancelEntryAbility(int entryIndex){
        CancelEntry(AbilityManager.AbilityInputEntries[entryIndex].Abilities);
    }

    private void ActivateEntry(List<GameComponentAbility> abilities){
        foreach (var ability in abilities){
            ability.AbilitySpec.Runner = this;
            StartCoroutine(ability.AbilitySpec.TryActivateAbility());
        }
    }
    private void CancelEntry(List<GameComponentAbility> abilities){
        foreach (var ability in abilities){
            ability.AbilitySpec.CancelAbility();
        }
    }

}