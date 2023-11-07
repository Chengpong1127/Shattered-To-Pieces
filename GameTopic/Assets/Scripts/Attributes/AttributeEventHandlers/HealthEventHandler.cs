using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Attribute Event Handler/HealthEventHandler")]
public class HealthEventHandler : ClampAttributeEventHandler
{
    override protected void ClampAttributeToMin(AttributeSystemComponent AttributeSystemComponent, AttributeScriptableObject primaryAttribute, AttributeScriptableObject maxAttribute)
    {
        base.ClampAttributeToMin(AttributeSystemComponent, primaryAttribute, maxAttribute);
        AttributeSystemComponent.GetAttributeValue(primaryAttribute, out var primaryAttributeValue);
        AttributeSystemComponent.GetAttributeValue(maxAttribute, out var minAttributeValue);
        if (primaryAttributeValue.CurrentValue <= minAttributeValue.CurrentValue){
            var owner = AttributeSystemComponent.GetComponent<Entity>();
            if (owner.IsInitialized) owner.Die();
        }
    }
    public override void AttributeChangedHandler(AttributeSystemComponent AttributeSystemComponent, AttributeScriptableObject attribute, AttributeValue prevAttributeValue, AttributeValue currentAttributeValue)
    {
        base.AttributeChangedHandler(AttributeSystemComponent, attribute, prevAttributeValue, currentAttributeValue);
        if(attribute == PrimaryAttribute){
            AttributeSystemComponent.GetAttributeValue(PrimaryAttribute, out var attributeValue);
            if (attributeValue.CurrentValue != prevAttributeValue.CurrentValue)
            {
                GameEvents.AttributeEvents.OnEntityHealthChanged.Invoke(AttributeSystemComponent.GetComponent<Entity>(), prevAttributeValue.CurrentValue, currentAttributeValue.CurrentValue);
            }
        }
    }
}