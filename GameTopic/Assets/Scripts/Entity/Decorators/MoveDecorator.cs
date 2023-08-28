using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;

public class MoveDecorator : Singleton<MoveDecorator>, IMonoDecorator<Entity>
{
    protected AttributeScriptableObject MovingVelocity;
    protected GameplayEffectScriptableObject Upspeed;
    public MoveDecorator(){
        MovingVelocity = ResourceManager.Instance.LoadAttribute("MovingVelocity");
        Upspeed = ResourceManager.Instance.LoadGameplayEffect("Upspeed");
    }
    public Entity Decorate(Entity entity)
    {
        entity.AttributeSystemComponent.AddAttributes(MovingVelocity);
        for(var i=0;i<5;i++)
        entity.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(entity.AbilitySystemCharacter.MakeOutgoingSpec(Upspeed));
        return entity;
    }
}