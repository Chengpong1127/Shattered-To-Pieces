using AbilitySystem.Authoring;
using AbilitySystem;



public abstract class RunnerAbilitySpec : EntityAbilitySpec
{
    public IAbilityRunner Runner { get; set; }
    protected RunnerAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
    {
    }
}



