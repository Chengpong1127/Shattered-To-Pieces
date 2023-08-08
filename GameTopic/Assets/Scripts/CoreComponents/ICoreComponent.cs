using System.Collections.Generic;
public interface ICoreComponent
{
    /// <summary>
    /// Get all of the ability list from this core component. 
    /// Core component should put all of its abilities in this dictionary at Awake(). Not Start().
    /// </summary>
    /// <value> The ability list. Ability name: Ability</value>
    public Dictionary<string, Ability> AllAbilities { get; }

    public IGameComponent OwnerGameComponent { get; set; }
}