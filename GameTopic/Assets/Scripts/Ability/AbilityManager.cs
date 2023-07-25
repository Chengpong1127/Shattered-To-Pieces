using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityManager
{
    public List<AbilityInputEntry> AbilityInputEntries { get; private set; } = new List<AbilityInputEntry>();
    /// <summary>
    /// The device that this input manager is recording.
    /// </summary>
    /// <value></value>
    public IDevice Device { get; set; }

    public readonly int AbilityInputEntryNumber;
    private Dictionary<Ability, bool> abilityInEntryStatus = new Dictionary<Ability, bool>();

    public AbilityManager(IDevice device, int abilityInputEntryNumber = 10){
        Debug.Assert(abilityInputEntryNumber > 0, "The number of ability input entry should be greater than 0");
        Debug.Assert(device != null, "The device should not be null");
        Device = device;
        AbilityInputEntryNumber = abilityInputEntryNumber;

        ReloadDeviceAbilities();
    }
    /// <summary>
    /// Reload the abilities of the device and put to out of entries.
    /// </summary>
    public void ReloadDeviceAbilities(){
        CreateAbilityInputEntries(AbilityInputEntryNumber);
        abilityInEntryStatus.Clear();
        UpdateDeviceAbilities();
    }
    /// <summary>
    /// Reload the abilities of the device, will not change the abilities in the input entries.
    /// </summary>
    public void UpdateDeviceAbilities(){
        var abilityList = GetDeviceCurrentAbilityList();

        foreach (var ability in abilityList)
        {
            if(!abilityInEntryStatus.ContainsKey(ability)){
                abilityInEntryStatus.Add(ability, false);
            }
        }
    }
    
    private void CreateAbilityInputEntries(int number){
        AbilityInputEntries.Clear();
        for (int i = 0; i < number; i++)
        {
            AbilityInputEntries.Add(new AbilityInputEntry());
        }
    }
    /// <summary>
    /// Set the key path of the input entry.
    /// </summary>
    /// <param name="entryID"> The index of the input entry.</param>
    /// <param name="path"> The key path of the input entry.</param>
    public void SetPath(int entryID, string path){
        Debug.Assert(entryID < AbilityInputEntries.Count, "index out of range");
        AbilityInputEntries[entryID].SetInputPath(path);
    }
    /// <summary>
    /// Assign an ability to the input entry.
    /// </summary>
    /// <param name="entryID"></param>
    /// <param name="ability"></param>
    public void SetAbilityToEntry(int entryID, Ability ability){
        removeAbilityFromEntry(ability);
        Debug.Assert(entryID < AbilityInputEntries.Count, "index out of range");
        var removed = AbilityInputEntries[entryID].AddAbility(ability);
        if(!abilityInEntryStatus.ContainsKey(ability)){
           Debug.LogWarning("The ability is not in the device");
           abilityInEntryStatus.Add(ability, false);
        }

        abilityInEntryStatus[ability] = true;
        if(removed != null){
           abilityInEntryStatus[removed] = false;
        }
    }
    /// <summary>
    /// Remove the ability from the input entry;
    /// </summary>
    /// <param name="ability"></param>
    public void SetAbilityOutOfEntry(Ability ability){
        removeAbilityFromEntry(ability);
        if(!abilityInEntryStatus.ContainsKey(ability)){
            Debug.LogWarning("The ability is not in the device");
            abilityInEntryStatus.Add(ability, false);
        }
        abilityInEntryStatus[ability] = false;
    }

    /// <summary>
    /// Get all the abilities that are not in the input entry.
    /// </summary>
    /// <returns></returns>
    public List<Ability> GetAbilitiesOutOfEntry(){
        return abilityInEntryStatus
            .Where(x => !x.Value)
            .Select(x => x.Key)
            .ToList();
    }
    private List<Ability> GetDeviceCurrentAbilityList(){
        return Device.getAbilityList();
    }

    private void removeAbilityFromEntry(Ability ability){
        foreach (var entry in AbilityInputEntries)
        {
            if(entry.ContainsAbility(ability)){
                entry.RemoveAbility(ability);
            }
        }
    }

}
