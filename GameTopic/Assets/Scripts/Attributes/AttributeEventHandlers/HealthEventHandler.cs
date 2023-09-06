using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Attribute Event Handler/HealthEventHandler")]
public class HealthEventHandler : ClampAttributeEventHandler
{
    private Entity owner;
    public override void PreAttributeChange(AttributeSystemComponent attributeSystem, List<AttributeValue> prevAttributeValues, ref List<AttributeValue> currentAttributeValues){
        base.PreAttributeChange(attributeSystem, prevAttributeValues, ref currentAttributeValues);
        if (attributeSystem.GetAttributeValue(PrimaryAttribute, out var primaryValue) && attributeSystem.GetAttributeValue(MinAttribute, out var minValue)){
            if (primaryValue.CurrentValue <= minValue.CurrentValue){
                owner = attributeSystem.GetComponent<Entity>();
                owner.Die();
            }
        }
            
    }
}