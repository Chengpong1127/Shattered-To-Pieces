using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;

public class MoveDecorator : Singleton<MoveDecorator>, IMonoDecorator<Entity>
{
    protected AttributeScriptableObject MovingVelocity;
    public MoveDecorator(){
        MovingVelocity = ResourceManager.Instance.LoadAttribute("MovingVelocity");
    }
    public Entity Decorate(Entity entity)
    {
        entity.AttributeSystemComponent.AddAttributes(MovingVelocity);
        return entity;
    }
}