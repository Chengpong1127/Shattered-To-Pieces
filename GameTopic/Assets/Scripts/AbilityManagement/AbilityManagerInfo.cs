using System.Collections.Generic;

public class AbilityManagerInfo: IInfo{
    public string[] EntryPaths;
    public List<(int, int)>[] EntryAbilities;
    public List<(int, int)> OutOfEntryAbilities;

    public AbilityManagerInfo(){
        
    }
    public AbilityManagerInfo(AbilityManager manager, Dictionary<IGameComponent, int> componentIDMap){
        EntryPaths = new string[manager.AbilityInputEntryNumber];
        EntryAbilities = new List<(int, int)>[manager.AbilityInputEntryNumber];
        OutOfEntryAbilities = new();
        for(int i = 0; i < manager.AbilityInputEntryNumber; i++){
            EntryPaths[i] = manager.AbilityInputEntries[i].InputPath;
            EntryAbilities[i] = new();
            foreach(var ability in manager.AbilityInputEntries[i].Abilities){
                EntryAbilities[i].Add((componentIDMap[ability.OwnerGameComponent.OwnerGameComponent], ability.AbilityIndex));
            }
        }
        
        foreach(var ability in manager.GetAbilitiesOutOfEntry()){
            OutOfEntryAbilities.Add((componentIDMap[ability.OwnerGameComponent.OwnerGameComponent], ability.AbilityIndex));
        }

    }

}