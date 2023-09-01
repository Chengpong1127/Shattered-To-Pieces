using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AbilityManager: IEnumerable<GameComponentAbility>
{
    /// <summary>
    /// Triggered after the ability is set to an entry.
    /// </summary>
    public event Action<GameComponentAbility> OnSetAbilityToEntry;
    /// <summary>
    /// Triggered after the ability is set to out of an entry.
    /// </summary>
    public event Action<GameComponentAbility> OnSetAbilityOutOfEntry;
    /// <summary>
    /// Triggered after setting binding
    /// </summary>
    public event Action<int, string> OnSetBinding;


    public List<AbilityInputEntry> AbilityInputEntries { get; private set; } = new();
    /// <summary>
    /// The device that this input manager is recording.
    /// </summary>
    /// <value></value>
    public IDevice Device { get; set; }

    public int AbilityInputEntryNumber = 10;
    private readonly Dictionary<GameComponentAbility, bool> abilityInEntryStatus = new();

    public AbilityManager(IDevice device, int abilityInputEntryNumber = 10){
        Debug.Assert(abilityInputEntryNumber > 0, "The number of ability input entry should be greater than 0");
        Debug.Assert(device != null, "The device should not be null");
        Device = device;
        AbilityInputEntryNumber = abilityInputEntryNumber;

        ReloadDeviceAbilities();
    }

    public AbilityManager(IDevice device, AbilityManagerInfo info, Dictionary<int, IGameComponent> componentMap){
        Load(device, info, componentMap);
    }
    public void Load(IDevice device, AbilityManagerInfo info, Dictionary<int, IGameComponent> componentMap){
        if (device == null) throw new ArgumentNullException(nameof(device));
        if (componentMap == null) throw new ArgumentNullException(nameof(componentMap));
        if (info != null && info.EntryPaths.Count() != info.EntryAbilities.Count()) throw new ArgumentException("The number of entry paths and entry abilities should be the same");

        GameComponentAbility getAbility(int componentID, int abilityIndex){
            var component = componentMap[componentID];
            Debug.Assert(component != null, "The component should not be null");
            var ability = component.CoreComponent.GameComponentAbilities[abilityIndex];
            
            Debug.Assert(ability != null, "The ability should not be null");
            return ability;
        }

        Device = device;
        if (info != null){
            AbilityInputEntryNumber = info.EntryPaths.Count();
            CreateAbilityInputEntries(AbilityInputEntryNumber);
            abilityInEntryStatus.Clear();
            for (int i = 0; i < AbilityInputEntryNumber; i++)
            {
                SetBinding(i, info.EntryPaths[i]);
            }
            foreach (var ability in GetDeviceCurrentAbilityList())
            {
                abilityInEntryStatus.Add(ability, true);
            }
            foreach (var (componentID, abilityIndex) in info.OutOfEntryAbilities)
            {
                var ability = getAbility(componentID, abilityIndex);
                abilityInEntryStatus[ability] = false;
            }

            for (int i = 0; i < AbilityInputEntryNumber; i++)
            {
                foreach (var (componentID, abilityIndex) in info.EntryAbilities[i])
                {
                    var ability = getAbility(componentID, abilityIndex);
                    SetAbilityToEntry(i, ability);
                }
            }
        }
        else{
            ReloadDeviceAbilities();
        }
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

        var removedList = new List<GameComponentAbility>();
        foreach (var ability in abilityInEntryStatus.Keys){
            if(!abilityList.Contains(ability)){
                RemoveAbilityFromEntry(ability);
                removedList.Add(ability);
            }
        }
        foreach (var ability in removedList){
            abilityInEntryStatus.Remove(ability);
        }

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
    public void SetBinding(int entryID, string path){
        AbilityInputEntries[entryID].SetInputPath(path);
        OnSetBinding?.Invoke(entryID, path);
        GameEvents.AbilityManagerEvents.OnSetBinding.Invoke(entryID, path);
    }
    /// <summary>
    /// Assign an ability to the input entry.
    /// </summary>
    /// <param name="entryID"></param>
    /// <param name="ability"></param>
    public void SetAbilityToEntry(int entryID, GameComponentAbility ability){
        RemoveAbilityFromEntry(ability);
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
        OnSetAbilityToEntry?.Invoke(ability);
        GameEvents.AbilityManagerEvents.OnSetAbilityToEntry.Invoke(ability);
    }
    /// <summary>
    /// Remove the ability from the input entry;
    /// </summary>
    /// <param name="ability"></param>
    public void SetAbilityOutOfEntry(GameComponentAbility ability){
        RemoveAbilityFromEntry(ability);
        if(!abilityInEntryStatus.ContainsKey(ability)){
            Debug.LogWarning("The ability is not in the device");
            abilityInEntryStatus.Add(ability, false);
        }
        abilityInEntryStatus[ability] = false;
        OnSetAbilityOutOfEntry?.Invoke(ability);
        GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry.Invoke(ability);
    }

    /// <summary>
    /// Get all the abilities that are not in the input entry.
    /// </summary>
    /// <returns></returns>
    public List<GameComponentAbility> GetAbilitiesOutOfEntry(){
        return abilityInEntryStatus
            .Where(x => !x.Value)
            .Select(x => x.Key)
            .ToList();
    }

    public GameComponentAbility[] GetDeviceCurrentAbilityList(){
        return Device.GetAbilityData();
    }


    private void RemoveAbilityFromEntry(GameComponentAbility ability){
        foreach (var entry in AbilityInputEntries)
        {
            if(entry.ContainsAbility(ability)){
                entry.RemoveAbility(ability);
            }
        }
    }

    public IEnumerator<GameComponentAbility> GetEnumerator()
    {
        return AbilityInputEntries.SelectMany(x => x).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
