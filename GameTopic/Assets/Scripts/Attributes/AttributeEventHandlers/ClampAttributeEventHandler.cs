using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Attribute Event Handler/Clamp Attribute")]
public class ClampAttributeEventHandler : AbstractAttributeEventHandler
{

    [SerializeField]
    protected AttributeScriptableObject PrimaryAttribute;
    [SerializeField]
    protected AttributeScriptableObject MaxAttribute;
    [SerializeField]
    protected AttributeScriptableObject MinAttribute;

    public override void AttributeChangedHandler(AttributeSystemComponent AttributeSystemComponent, AttributeScriptableObject attribute, AttributeValue prevAttributeValue, AttributeValue currentAttributeValue)
    {
        if (attribute == PrimaryAttribute){
            if (MaxAttribute != null)
            {
                ClampAttributeToMax(AttributeSystemComponent, PrimaryAttribute, MaxAttribute);
            }
            if (MinAttribute != null)
            {
                ClampAttributeToMin(AttributeSystemComponent, PrimaryAttribute, MinAttribute);
            }
        }
    }

    protected virtual void ClampAttributeToMax(AttributeSystemComponent AttributeSystemComponent, AttributeScriptableObject primaryAttribute, AttributeScriptableObject maxAttribute)
    {
        AttributeSystemComponent.GetAttributeValue(primaryAttribute, out var primaryAttributeValue);
        AttributeSystemComponent.GetAttributeValue(maxAttribute, out var maxAttributeValue);
        if (primaryAttributeValue.CurrentValue > maxAttributeValue.CurrentValue)
        {
            primaryAttributeValue.CurrentValue = maxAttributeValue.CurrentValue;
            AttributeSystemComponent.ResetAttributeModifiers(primaryAttribute);
        }
        primaryAttributeValue.BaseValue = Mathf.Min(primaryAttributeValue.BaseValue, maxAttributeValue.BaseValue);
        AttributeSystemComponent.SetAttributeValue(primaryAttribute, primaryAttributeValue);
    }
    protected virtual void ClampAttributeToMin(AttributeSystemComponent AttributeSystemComponent, AttributeScriptableObject primaryAttribute, AttributeScriptableObject minAttribute)
    {
        AttributeSystemComponent.GetAttributeValue(primaryAttribute, out var primaryAttributeValue);
        AttributeSystemComponent.GetAttributeValue(minAttribute, out var minAttributeValue);
        if (primaryAttributeValue.CurrentValue < minAttributeValue.CurrentValue)
        {
            primaryAttributeValue.CurrentValue = minAttributeValue.CurrentValue;
            AttributeSystemComponent.ResetAttributeModifiers(primaryAttribute);
        }
        primaryAttributeValue.BaseValue = Mathf.Max(primaryAttributeValue.BaseValue, minAttributeValue.BaseValue);
        AttributeSystemComponent.SetAttributeValue(primaryAttribute, primaryAttributeValue);
    }
}
