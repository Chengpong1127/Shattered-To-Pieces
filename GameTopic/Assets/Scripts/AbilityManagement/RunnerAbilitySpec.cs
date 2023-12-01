using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;



public abstract class RunnerAbilitySpec : EntityAbilitySpec
{
    public AbilityRunner Runner { get; set; }
    public EnergyManager EnergyManager { get; set; }
    protected RunnerAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
    {
    }

    public override bool CanActivateAbility()
    {
        if (EnergyManager == null) 
            return base.CanActivateAbility();
        else
            return base.CanActivateAbility() && EnergyManager.HasEnergy(Ability.EnergyCost);
    }

    protected override void PreActivate()
    {
        if (EnergyManager != null)
            EnergyManager.CostEnergy(Ability.EnergyCost);
    }
}



