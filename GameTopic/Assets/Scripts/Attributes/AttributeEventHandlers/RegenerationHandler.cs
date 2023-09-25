using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;


[CreateAssetMenu(menuName = "Gameplay Ability System/Attribute Event Handler/RegenerationHandler")]
public class RegenerationHandler : AbstractAttributeEventHandler
{
    [SerializeField]
    private AttributeScriptableObject HealthAttribute;
    [SerializeField]
    private GameplayEffectScriptableObject StopRegenerationEffect;
    public override void PreAttributeChange(AttributeSystemComponent attributeSystem, List<AttributeValue> prevAttributeValues, ref List<AttributeValue> currentAttributeValues)
    {
        var attributeCacheDict = attributeSystem.mAttributeIndexCache;
        if (attributeCacheDict.TryGetValue(HealthAttribute, out var primaryAttributeIndex))
        {
            var prevValue = prevAttributeValues[primaryAttributeIndex].CurrentValue;
            var currentValue = currentAttributeValues[primaryAttributeIndex].CurrentValue;

            if (currentValue < prevValue)
            {
                var owner = attributeSystem.GetComponent<Entity>();
                GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(owner, owner, StopRegenerationEffect);
            }
        }
    }
}