using UnityEngine;
using System.Collections.Generic;


public class AbilityRunner: MonoBehaviour{
    public AbilityManager AbilityManager { get; set; }
    private HashSet<int> RunningAbilitySet = new HashSet<int>();

    public void StartAbility(int entryIndex){
        Debug.Assert(AbilityManager != null, "The ability manager should not be null");
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
        Debug.Assert(RunningAbilitySet.Contains(entryIndex), "The ability is not running");
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