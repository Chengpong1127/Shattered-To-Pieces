using AbilitySystem;
using AttributeSystem.Components;
using AttributeSystem.Authoring;
using AbilitySystem.Authoring;
using UnityEngine;
using System;
using UnityEngine.InputSystem.LowLevel;

public class GameEffectManager{
    public void Enable(){
        GameEvents.GameEffectManagerEvents.RequestModifyAttribute += ModifyAttribute;
        GameEvents.GameEffectManagerEvents.RequestGiveGameEffect += ApplyGameplayEffect;
        GameEvents.GameEffectManagerEvents.RequestRemoveGameEffect += RemoveGameplayEffect;
    }
    public void Disable(){
        GameEvents.GameEffectManagerEvents.RequestModifyAttribute -= ModifyAttribute;
        GameEvents.GameEffectManagerEvents.RequestGiveGameEffect -= ApplyGameplayEffect;
        GameEvents.GameEffectManagerEvents.RequestRemoveGameEffect -= RemoveGameplayEffect;
    }
    private void ModifyAttribute(Entity sender, Entity receiver, GameplayEffectModifier modifier){
        var effect = ResourceManager.Instance.LoadEmptyGameplayEffect();
        effect.gameplayEffect.Modifiers = new GameplayEffectModifier[]{modifier};
        effect.gameplayEffect.DurationPolicy = EDurationPolicy.Instant;
        ApplyGameplayEffect(sender, receiver, effect);
    }
    private void ApplyGameplayEffect(Entity sender, Entity receiver, GameplayEffectScriptableObject gameplayEffect){
        var spec = sender.AbilitySystemCharacter.MakeOutgoingSpec(gameplayEffect);
        receiver.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(spec);
    }
    private void RemoveGameplayEffect(Entity receiver, GameplayEffectScriptableObject gameplayEffect){
        receiver.AbilitySystemCharacter.RemoveGameplayEffect(gameplayEffect);
    }
}