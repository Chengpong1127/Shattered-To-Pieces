using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;

public class AttackDecorator : Singleton<AttackDecorator>, IMonoDecorator<Entity>
{
    protected AttributeScriptableObject AttackPoint;
    public AttackDecorator()
    {
        AttackPoint = ResourceManager.Instance.LoadAttribute("AttackPoint");
    }
    public Entity Decorate(Entity entity)
    {
        entity.AttributeSystemComponent.AddAttributes(AttackPoint);
        return entity;
    }
}