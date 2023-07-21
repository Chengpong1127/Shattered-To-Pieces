using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityInputManager
{
    public List<AbilityInputEntry> AbilityInputEntries { get; private set; } = new List<AbilityInputEntry>();
    /// <summary>
    /// The device that this input manager is recording.
    /// </summary>
    /// <value></value>
    public IDevice Device { get; set; }

    public readonly int AbilityInputEntryNumber;
    private Dictionary<Ability, bool> abilityInEntryStatus = new Dictionary<Ability, bool>();

    public AbilityInputManager(IDevice device, int abilityInputEntryNumber = 10){
        Device = device;
        AbilityInputEntryNumber = abilityInputEntryNumber;
        CreateAbilityInputEntries(AbilityInputEntryNumber);
        var abilityList = GetDeviceCurrentAbilityList();
        foreach (var ability in abilityList)
        {
            abilityInEntryStatus.Add(ability, false);
        }
        

    }
    
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
    public void SetAbility(int index, Ability ability){
        Debug.Assert(index < AbilityInputEntries.Count, "index out of range");
        AbilityInputEntries[index].AddAbility(ability);
    }

    public void SetAbilityOutOfEntry(Ability ability){
        abilityInEntryStatus[ability] = false;
    }

    public List<Ability> GetAbilitiesOutOfEntry(){
        return abilityInEntryStatus
            .Where(x => !x.Value)
            .Select(x => x.Key)
            .ToList();
    }
    private List<Ability> GetDeviceCurrentAbilityList(){
        return Device.getAbilityList();
    }

}
