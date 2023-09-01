public interface IAbilityRunner{
    /// <summary>
    /// Start the ability by its name.
    /// </summary>
    /// <param name="abilityName"> The name of the ability.</param>
    /// <param name="specificOwner"> The owner of the ability. If null, start the ability with the name. If not null, start the ability with the specific owner.</param>
    /// <param name="all"> Whether to start all the abilities with the same name. If false, only start the first one.</param>
    public void StartSingleAbility(string abilityName, ICoreComponent specificOwner = null, bool all = false);
    /// <summary>
    /// Start the ability by its index in the input entry.
    /// </summary>
    /// <param name="abilityName"> The name of the ability.</param>
    /// <param name="specificOwner"> The owner of the ability. If null, start the ability with the name. If not null, start the ability with the specific owner.</param>
    /// <param name="all"> Whether to start all the abilities with the same name. If false, only start the first one.</param>
    public void CancelSingleAbility(string abilityName, ICoreComponent specificOwner = null, bool all = false);
}