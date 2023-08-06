using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AbilityRunner: MonoBehaviour{
    public AbilityManager AbilityManager { get; set; }
    private readonly HashSet<int> RunningAbilitySet = new();
    public InputAction[] AbilityActions { get; set; }
    private void Start() {
        Debug.Assert(AbilityManager != null, "The ability manager should be set before Start()");
        if (AbilityActions != null)
        {
            for (int i = 0; i < AbilityActions.Length; i++)
            {
                var abilityNumber = i;
                AbilityActions[abilityNumber].AddBinding(AbilityManager.AbilityInputEntries[abilityNumber].InputPath);
                AbilityActions[abilityNumber].started += ctx => StartAbility(abilityNumber);
                AbilityActions[abilityNumber].canceled += ctx => EndAbility(abilityNumber);
            }
        }
        
    }
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