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

    public override void AttributeChangedHandler(AttributeSystemComponent AttributeSystemComponent, AttributeScriptableObject attribute, AttributeValue prevAttributeValue, AttributeValue currentAttributeValue)
    {
        if (attribute == HealthAttribute)
        {
            if (currentAttributeValue.CurrentValue < prevAttributeValue.CurrentValue)
            {
                var owner = AttributeSystemComponent.GetComponent<Entity>();
                GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(owner, owner, StopRegenerationEffect);
            }
        }
    }
}