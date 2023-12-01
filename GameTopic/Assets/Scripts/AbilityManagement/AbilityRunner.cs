using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class AbilityRunner: MonoBehaviour{
    public AbilityManager AbilityManager { get; private set; }
    public EnergyManager EnergyManager { get; private set; }
    public ulong OwnerPlayerID { get; private set; }
    public static AbilityRunner CreateInstance(GameObject where, AbilityManager abilityManager, ulong playerID){
        if (where == null){
            throw new ArgumentNullException(nameof(where));
        }
        var abilityRunner = where.AddComponent<AbilityRunner>();
        abilityRunner.AbilityManager = abilityManager ?? throw new ArgumentNullException(nameof(abilityManager));
        abilityRunner.OwnerPlayerID = playerID;
        return abilityRunner;
    }
    void Awake()
    {
        EnergyManager = GetComponent<EnergyManager>();
    }
    public void StartEntryAbility(int entryIndex){
        ActivateEntry(AbilityManager.AbilityInputEntries[entryIndex].Abilities);
    }
    public void CancelEntryAbility(int entryIndex){
        CancelEntry(AbilityManager.AbilityInputEntries[entryIndex].Abilities);
    }

    private void ActivateEntry(List<GameComponentAbility> abilities){
        foreach (var ability in abilities){
            ability.AbilitySpec.Runner = this;
            ability.AbilitySpec.EnergyManager = EnergyManager;
            StartCoroutine(ability.AbilitySpec.TryActivateAbility());
        }
    }
    private void CancelEntry(List<GameComponentAbility> abilities){
        foreach (var ability in abilities){
            ability.AbilitySpec.CancelAbility();
        }
    }

}