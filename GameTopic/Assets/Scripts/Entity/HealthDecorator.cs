using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;
public class HealthEntityDecorator : Singleton<HealthEntityDecorator>, IMonoDecorator<Entity>
{
    protected AttributeScriptableObject MaxHealthAttribute;
    protected AttributeScriptableObject MinHealthAttribute;
    protected AttributeScriptableObject HealthAttribute;

    protected AbstractAttributeEventHandler ClampHealthEventHandler;
    public HealthEntityDecorator(){
        HealthAttribute = ResourceManager.Instance.LoadAttribute("Health");
        MaxHealthAttribute = ResourceManager.Instance.LoadAttribute("MaxHealth");
        MinHealthAttribute = ResourceManager.Instance.LoadAttribute("MinHealth");

        ClampHealthEventHandler = ResourceManager.Instance.LoadAttributeEventHandler("ClampHealthEventHandler");
    }
    public Entity Decorate(Entity entity)
    {
        entity.AttributeSystemComponent.AddAttributes(HealthAttribute);
        entity.AttributeSystemComponent.AddAttributes(MaxHealthAttribute);
        entity.AttributeSystemComponent.AddAttributes(MinHealthAttribute);

        return entity;
    }
}