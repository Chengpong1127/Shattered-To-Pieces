using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using System.Collections;

public class AbilityInputEntry: IEnumerable<GameComponentAbility>{
    /// <summary>
    /// The path of the input, for example, the space of keyboard, the left mouse button, etc.
    /// </summary>
    /// <value></value>
    public string InputPath { get; private set; } = "";
    public static readonly int AbilityNumber = 3;
    public List<GameComponentAbility> Abilities = new();
    public AbilityInputEntry(){
    }
    /// <summary>
    /// Set the input path of this entry
    /// </summary>
    /// <param name="inputPath"></param>
    public void SetInputPath(string inputPath){
        InputPath = inputPath;
    }
    public void SetAbility(int index, GameComponentAbility ability){
        Debug.Assert(index < Abilities.Count, "index out of range");
        Abilities[index] = ability;
    }

    /// <summary>
    /// Add an ability to the entry, and return the removed ability.
    /// </summary>
    /// <param name="ability"> The ability to be added.</param>
    /// <returns> The removed ability.</returns>
    public GameComponentAbility AddAbility(GameComponentAbility ability){
        Abilities.Insert(0, ability);
        if(Abilities.Count > AbilityNumber){
            GameComponentAbility removedAbility = Abilities[^1];
            Abilities.RemoveAt(Abilities.Count - 1);
            return removedAbility;
        }
        return null;
    }
    public void RemoveAbility(GameComponentAbility ability){
        Abilities.Remove(ability);
    }
    public bool ContainsAbility(GameComponentAbility ability){
        return Abilities.Contains(ability);
    }
    public void Reset(){
        for(int i = 0; i < Abilities.Count; i++){
            Abilities[i] = null;
        }
    }

    public IEnumerator<GameComponentAbility> GetEnumerator()
    {
        return Abilities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
