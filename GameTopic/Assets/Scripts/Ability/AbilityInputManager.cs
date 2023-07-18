using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInputManager
{
    public AbilityInputManager(){
        CreateAbilityInputEntries(10);
    }
    public List<AbilityInputEntry> AbilityInputEntries { get; private set; } = new List<AbilityInputEntry>();
    public void CreateAbilityInputEntries(int number){
        for (int i = 0; i < number; i++)
        {
            AbilityInputEntries.Add(new AbilityInputEntry());
        }
    }
    public void SetPath(int index, string path){
        Debug.Assert(index < AbilityInputEntries.Count, "index out of range");
        AbilityInputEntries[index].SetInputPath(path);
    }
    public void SetAbility(int index, int abilityEntryIndex, Ability ability){
        Debug.Assert(index < AbilityInputEntries.Count, "index out of range");
        AbilityInputEntries[index].SetAbility(abilityEntryIndex, ability);
    }

}
