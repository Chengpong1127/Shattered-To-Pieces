using System.Collections.Generic;
using AbilitySystem.Authoring;

public interface IDevice{
    public IGameComponent RootGameComponent { get; }
    /// <summary>
    /// Get all of the ability list of this device.
    /// </summary>
    /// <returns></returns>
    public GameComponentAbility[] GetAbilityData();
}
