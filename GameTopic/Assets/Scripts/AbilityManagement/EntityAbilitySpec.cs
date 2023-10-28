using System.Collections;
using AbilitySystem.Authoring;
using AbilitySystem;


public abstract class EntityAbilitySpec: AbstractAbilitySpec{
    protected Entity SelfEntity;
    protected EntityAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
    {
        SelfEntity = owner.GetComponent<Entity>() ?? throw new System.ArgumentNullException("There is no Entity component in the game object");
    }

    public override bool CanActivateAbility()
    {
        if (SelfEntity is GameComponent gameComponent){
            return base.CanActivateAbility() && !gameComponent.IsSelected.Value;
        }else{
            return base.CanActivateAbility();
        }
    }

    protected override IEnumerator PreActivate()
    {
        if (Ability.Cooldown != null) {
            var cooldownSpec = Owner.MakeOutgoingSpec(Ability.Cooldown);
            Owner.ApplyGameplayEffectSpecToSelf(cooldownSpec);
        }
        if (Ability.Cost != null){
            var costSpec = Owner.MakeOutgoingSpec(Ability.Cost);
            Owner.ApplyGameplayEffectSpecToSelf(costSpec);
        }
        yield return null;
    }
    public override bool CheckGameplayTags(){
        return AscHasAllTags(Owner, this.Ability.AbilityTags.OwnerTags.RequireTags)
                && AscHasNoneTags(Owner, this.Ability.AbilityTags.OwnerTags.IgnoreTags)
                && AscHasAllTags(Owner, this.Ability.AbilityTags.SourceTags.RequireTags)
                && AscHasNoneTags(Owner, this.Ability.AbilityTags.SourceTags.IgnoreTags)
                && AscHasNoneTags(Owner, this.Ability.AbilityTags.BlockAbilitiesWithTags);
    }
}