using System.Collections.Generic;

public class AbilityManagerInfo{
    public string[] EntryPaths;
    public List<(int, string)>[] Abilities;

    public AbilityManagerInfo(string[] entryPaths, List<(int, string)>[] abilities){
        EntryPaths = entryPaths;
        Abilities = abilities;
    }

}