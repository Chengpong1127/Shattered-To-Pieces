using AbilitySystem.Authoring;
using AbilitySystem;


public abstract class EntityAbilitySpec: AbstractAbilitySpec{
    protected Entity SelfEntity;
    protected EntityAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
    {
        SelfEntity = owner.GetComponent<Entity>() ?? throw new System.ArgumentNullException("There is no Entity component in the game object");
    }
}