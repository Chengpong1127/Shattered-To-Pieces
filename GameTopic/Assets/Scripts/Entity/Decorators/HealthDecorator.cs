using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
public class HealthEntityDecorator : Singleton<HealthEntityDecorator>, IMonoDecorator<Entity>
{
    protected AttributeScriptableObject MaxHealthAttribute;
    protected AttributeScriptableObject MinHealthAttribute;
    protected AttributeScriptableObject HealthAttribute;

    protected AbstractAttributeEventHandler ClampHealthEventHandler;
    protected GameplayEffectScriptableObject HealInitialEffect;
    public HealthEntityDecorator(){
        HealthAttribute = ResourceManager.Instance.LoadAttribute("Health");
        MaxHealthAttribute = ResourceManager.Instance.LoadAttribute("MaxHealth");
        MinHealthAttribute = ResourceManager.Instance.LoadAttribute("MinHealth");
        ClampHealthEventHandler = ResourceManager.Instance.LoadAttributeEventHandler("ClampHealthEventHandler");
        HealInitialEffect = ResourceManager.Instance.LoadGameplayEffect("HealthInit");
    }
    public Entity Decorate(Entity entity)
    {
        entity.AttributeSystemComponent.AddAttributes(HealthAttribute, MaxHealthAttribute, MinHealthAttribute);

        entity.AttributeSystemComponent.AddAttributeEventHandlers(ClampHealthEventHandler);
        entity.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(entity.AbilitySystemCharacter.MakeOutgoingSpec(HealInitialEffect));
        return entity;
    }
}