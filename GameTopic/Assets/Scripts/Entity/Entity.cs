using System.Collections.Generic;
using System;
using System.ComponentModel;
using AttributeSystem.Components;
using AbilitySystem;

public abstract class Entity: BaseEntity{
    public AttributeSystemComponent AttributeSystemComponent;
    public AbilitySystemCharacter AbilitySystemCharacter;
    protected virtual void Awake() {
        AttributeSystemComponent = GetComponent<AttributeSystemComponent>();
        if (AttributeSystemComponent == null) {
            AttributeSystemComponent = gameObject.AddComponent<AttributeSystemComponent>();
        }
        AbilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
        if (AbilitySystemCharacter == null) {
            AbilitySystemCharacter = gameObject.AddComponent<AbilitySystemCharacter>();
        }
        AbilitySystemCharacter.AttributeSystem = AttributeSystemComponent;
    }



    protected Dictionary<Type, IStatus> _statuses = new Dictionary<Type, IStatus>();
    public BuffManager BuffManager= new BuffManager();

    private void Update() {
        foreach (Buff buff in BuffManager.buffs) {
            buff.Update();
        }
    }

    public IStatus GetStatus(Type type) {
        return type.IsSubclassOf(typeof(IStatus)) && _statuses.ContainsKey(type) ? _statuses[type] : null;
    }
}

#region exampleInheritance
public class ExampleEntity : Entity {
    ExampleEntity() {
        _statuses.Add(typeof(HealthStatus), new HealthStatus());
        _statuses.Add(typeof(SingleTarget), new SingleTarget());
        _statuses.Add(typeof(AttackStatus), new AttackStatus());
        
    }
}
#endregion