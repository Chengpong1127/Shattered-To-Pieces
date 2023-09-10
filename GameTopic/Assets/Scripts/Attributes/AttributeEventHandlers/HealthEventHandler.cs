using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Attribute Event Handler/HealthEventHandler")]
public class HealthEventHandler : ClampAttributeEventHandler
{
    public override void PreAttributeChange(AttributeSystemComponent attributeSystem, List<AttributeValue> prevAttributeValues, ref List<AttributeValue> currentAttributeValues){
        bool changed = false;
        if (attributeSystem.mAttributeIndexCache.TryGetValue(PrimaryAttribute, out var healthIndex)){
            if (currentAttributeValues[healthIndex].CurrentValue != prevAttributeValues[healthIndex].CurrentValue){
                changed = true;
            }
        }
        base.PreAttributeChange(attributeSystem, prevAttributeValues, ref currentAttributeValues);
        if (changed && attributeSystem.mAttributeIndexCache.TryGetValue(PrimaryAttribute, out healthIndex)){
            GameEvents.AttributeEvents.OnEntityHealthChanged.Invoke(attributeSystem.GetComponent<Entity>(), prevAttributeValues[healthIndex].CurrentValue, currentAttributeValues[healthIndex].CurrentValue);
        }



        if (attributeSystem.GetAttributeValue(PrimaryAttribute, out var primaryValue) && attributeSystem.GetAttributeValue(MinAttribute, out var minValue)){
            var owner = attributeSystem.GetComponent<Entity>();
            if (primaryValue.CurrentValue <= minValue.CurrentValue && owner.IsInitialized){
                owner.Die();
            }
        }
            
    }
}