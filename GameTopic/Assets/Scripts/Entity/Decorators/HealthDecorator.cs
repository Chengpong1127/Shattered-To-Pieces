using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
public class HealthEntityDecorator : Singleton<HealthEntityDecorator>, IMonoDecorator<Entity>
{
    protected AttributeScriptableObject MaxHealthAttribute;
    protected AttributeScriptableObject MinHealthAttribute;
    protected AttributeScriptableObject HealthAttribute;

    protected AbstractAttributeEventHandler HealthEventHandler;
    protected GameplayEffectScriptableObject HealInitialEffect;
    protected AbstractAttributeEventHandler ShowDamageNumbers;

    public HealthEntityDecorator(){
        HealthAttribute = ResourceManager.Instance.LoadAttribute("Health");
        MaxHealthAttribute = ResourceManager.Instance.LoadAttribute("MaxHealth");
        MinHealthAttribute = ResourceManager.Instance.LoadAttribute("MinHealth");
        HealthEventHandler = ResourceManager.Instance.LoadAttributeEventHandler("HealthEventHandler");
        HealInitialEffect = ResourceManager.Instance.LoadGameplayEffect("HealthInit");
        ShowDamageNumbers = ResourceManager.Instance.LoadAttributeEventHandler("Damage Numbers");
    }
    public Entity Decorate(Entity entity)
    {
        entity.AttributeSystemComponent.AddAttributes(HealthAttribute, MaxHealthAttribute, MinHealthAttribute);
        entity.AttributeSystemComponent.AddAttributeEventHandlers(HealthEventHandler);
        entity.AttributeSystemComponent.AddAttributeEventHandlers(ShowDamageNumbers);
        entity.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(entity.AbilitySystemCharacter.MakeOutgoingSpec(HealInitialEffect));
        return entity;
    }
}