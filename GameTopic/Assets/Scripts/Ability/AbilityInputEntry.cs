using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInputEntry{
    /// <summary>
    /// The path of the input, for example, the space of keyboard, the left mouse button, etc.
    /// </summary>
    /// <value></value>
    public string InputPath { get; private set; } = "";
    public static readonly int AbilityNumber = 3;
    public List<Ability> Abilities = new List<Ability>();
    public AbilityInputEntry(){
        for (int i = 0; i < AbilityNumber; i++)
        {
            Abilities.Add(null);
        }
    }
    /// <summary>
    /// Set the input path of this entry
    /// </summary>
    /// <param name="inputPath"></param>
    public void SetInputPath(string inputPath){
        InputPath = inputPath;
    }
    public void SetAbility(int index, Ability ability){
        Debug.Assert(index < Abilities.Count, "index out of range");
        Abilities[index] = ability;
    }

    /// <summary>
    /// Add an ability to the entry, and return the removed ability.
    /// </summary>
    /// <param name="ability"> The ability to be added.</param>
    /// <returns> The removed ability.</returns>
    public Ability AddAbility(Ability ability){
        Abilities.Insert(0, ability);
        Ability removedAbility = Abilities[Abilities.Count - 1];
        Abilities.RemoveAt(Abilities.Count - 1);
        return removedAbility;
    }
    /// <summary>
    /// Trigger all of the abilities in this entry.
    /// </summary>
    public void TriggerAllAbilities(){
        foreach (var ability in Abilities)
        {
            if(ability != null){
                ability.StartAbility();
            }
        }
    }
    /// <summary>
    /// Run all of the abilities in this entry. Call this function at each frame.
    /// </summary>
    public void RunAllAbilitiesForEachFrame(){
        foreach (var ability in Abilities)
        {
            if(ability != null){
                ability.RunAbility();
            }
        }
    }
    /// <summary>
    /// End all of the abilities in this entry.
    /// </summary>
    public void EndAllAbilities(){
        foreach (var ability in Abilities)
        {
            if(ability != null){
                ability.EndAbility();
            }
        }
    }
    public void Reset(){
        for(int i = 0; i < Abilities.Count; i++){
            Abilities[i] = null;
        }
    }

}
