using System.Collections;
using AbilitySystem.Authoring;
using AbilitySystem;


public abstract class EntityAbilitySpec: AbstractAbilitySpec{
    protected Entity SelfEntity;
    protected EntityAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
    {
        SelfEntity = owner.GetComponent<Entity>() ?? throw new System.ArgumentNullException("There is no Entity component in the game object");
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
}