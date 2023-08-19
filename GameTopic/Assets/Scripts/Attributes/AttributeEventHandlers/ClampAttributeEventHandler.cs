using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Attribute Event Handler/Clamp Attribute")]
public class ClampAttributeEventHandler : AbstractAttributeEventHandler
{

    [SerializeField]
    private AttributeScriptableObject PrimaryAttribute;
    [SerializeField]
    private AttributeScriptableObject MaxAttribute;
    [SerializeField]
    private AttributeScriptableObject MinAttribute;
    public override void PreAttributeChange(AttributeSystemComponent attributeSystem, List<AttributeValue> prevAttributeValues, ref List<AttributeValue> currentAttributeValues)
    {
        var attributeCacheDict = attributeSystem.mAttributeIndexCache;
        if (MaxAttribute != null)
            ClampAttributeToMax(PrimaryAttribute, MaxAttribute, currentAttributeValues, attributeCacheDict);
        if (MinAttribute != null)
            ClampAttributeToMin(PrimaryAttribute, MinAttribute, currentAttributeValues, attributeCacheDict);
        if (MaxAttribute == null && MinAttribute == null)
            Debug.LogWarning("ClampAttributeEventHandler: MaxAttribute and MinAttribute are both null, this event handler will do nothing.");
    }

    private void ClampAttributeToMax(AttributeScriptableObject Attribute1, AttributeScriptableObject Attribute2, List<AttributeValue> attributeValues, Dictionary<AttributeScriptableObject, int> attributeCacheDict)
    {
        if (attributeCacheDict.TryGetValue(Attribute1, out var primaryAttributeIndex)
            && attributeCacheDict.TryGetValue(Attribute2, out var maxAttributeIndex))
        {
            var primaryAttribute = attributeValues[primaryAttributeIndex];
            var maxAttribute = attributeValues[maxAttributeIndex];

            // Clamp current and base values
            if (primaryAttribute.CurrentValue > maxAttribute.CurrentValue) primaryAttribute.CurrentValue = maxAttribute.CurrentValue;
            if (primaryAttribute.BaseValue > maxAttribute.BaseValue) primaryAttribute.BaseValue = maxAttribute.BaseValue;
            attributeValues[primaryAttributeIndex] = primaryAttribute;
        }
    }
    private void ClampAttributeToMin(AttributeScriptableObject Attribute1, AttributeScriptableObject Attribute2, List<AttributeValue> attributeValues, Dictionary<AttributeScriptableObject, int> attributeCacheDict)
    {
        if (attributeCacheDict.TryGetValue(Attribute1, out var primaryAttributeIndex)
            && attributeCacheDict.TryGetValue(Attribute2, out var minAttributeIndex))
        {
            var primaryAttribute = attributeValues[primaryAttributeIndex];
            var minAttribute = attributeValues[minAttributeIndex];

            // Clamp current and base values
            if (primaryAttribute.CurrentValue < minAttribute.CurrentValue) primaryAttribute.CurrentValue = minAttribute.CurrentValue;
            if (primaryAttribute.BaseValue < minAttribute.BaseValue) primaryAttribute.BaseValue = minAttribute.BaseValue;
            attributeValues[primaryAttributeIndex] = primaryAttribute;
        }
    }
}
