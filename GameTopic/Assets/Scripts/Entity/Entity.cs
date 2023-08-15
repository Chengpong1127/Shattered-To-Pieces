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
}