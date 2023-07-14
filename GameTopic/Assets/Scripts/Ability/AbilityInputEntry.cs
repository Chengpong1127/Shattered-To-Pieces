using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInputEntry{
    /// <summary>
    /// The path of the input, for example, the space of keyboard, the left mouse button, etc.
    /// </summary>
    /// <value></value>
    public string InputPath { get; private set; } = "";
    public const int AbilityNumber = 3;
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
    public void RunAllAbilities(){
        foreach (var ability in Abilities)
        {
            if(ability != null){
                ability.Run();
            }
        }
    }
    public void Reset(){
        for(int i = 0; i < Abilities.Count; i++){
            Abilities[i] = null;
        }
    }

}
