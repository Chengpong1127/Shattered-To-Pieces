using AttributeSystem.Components;
using AbilitySystem;
using UnityEngine;

[RequireComponent(typeof(AttributeSystemComponent)), RequireComponent(typeof(AbilitySystemCharacter))]
public abstract class Entity: BaseEntity{
    public AttributeSystemComponent AttributeSystemComponent;
    public AbilitySystemCharacter AbilitySystemCharacter;
    protected virtual void Awake() {
        AttributeSystemComponent = GetComponent<AttributeSystemComponent>();
        AbilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
        AbilitySystemCharacter.AttributeSystem = AttributeSystemComponent;
    }
}