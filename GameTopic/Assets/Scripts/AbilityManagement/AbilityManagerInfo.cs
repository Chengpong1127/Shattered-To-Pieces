using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AbilityManagerInfo: IInfo{
    public string[] EntryPaths;
    public List<(int, int)>[] EntryAbilities;
    public List<(int, int)> OutOfEntryAbilities;

    public AbilityManagerInfo(){
        
    }
    public AbilityManagerInfo(AbilityManager manager, Dictionary<ulong, int> componentIDMap){
        EntryPaths = new string[manager.AbilityInputEntryNumber];
        EntryAbilities = new List<(int, int)>[manager.AbilityInputEntryNumber];
        OutOfEntryAbilities = new();
        for(int i = 0; i < manager.AbilityInputEntryNumber; i++){
            EntryPaths[i] = manager.AbilityInputEntries[i].InputPath;
            EntryAbilities[i] = new();
            foreach(var ability in manager.AbilityInputEntries[i].Abilities){
                EntryAbilities[i].Add((componentIDMap[ability.OwnerGameComponentID], ability.AbilityIndex));
            }
        }
        
        foreach(var ability in manager.GetAbilitiesOutOfEntry()){
            OutOfEntryAbilities.Add((componentIDMap[ability.OwnerGameComponentID], ability.AbilityIndex));
        }

    }
    public InputActionMap GetAbilityInputActionMap(){
        var map = new InputActionMap("AbilityInput");
        for(int i = 0; i < EntryPaths.Length; i++){
            var abilityIndex = i;
            string keyName = "Ability" + abilityIndex.ToString();
            InputAction action = map.AddAction(keyName);
            action.AddBinding(EntryPaths[abilityIndex]);
            action.started += (ctx) => {
                GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility.Invoke(abilityIndex);
            };
            action.canceled += (ctx) => {
                GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility.Invoke(abilityIndex);
            };
        }
        return map;
    }

}