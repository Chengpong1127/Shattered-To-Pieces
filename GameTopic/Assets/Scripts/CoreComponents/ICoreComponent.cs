using System.Collections.Generic;
public interface ICoreComponent
{
    /// <summary>
    /// Get all of the ability list from this core component.
    /// </summary>
    /// <value> The ability list. Ability name: Ability</value>
    public Dictionary<string, Ability> AllAbilities { get; }

    public IGameComponent OwnerGameComponent { get; set; }
}