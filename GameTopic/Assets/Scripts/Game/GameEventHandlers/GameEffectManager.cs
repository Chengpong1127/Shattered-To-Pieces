using AbilitySystem;
using AttributeSystem.Components;
using AttributeSystem.Authoring;
using AbilitySystem.Authoring;
using UnityEngine;
using System;
using UnityEngine.InputSystem.LowLevel;

public class GameEffectManager: BaseGameEventHandler{
    void OnEnable()
    {
        GameEvents.GameEffectManagerEvents.RequestModifyAttribute += ModifyAttribute;
        GameEvents.GameEffectManagerEvents.RequestGiveGameEffect += ApplyGameplayEffect;
    }
    void OnDisable()
    {
        GameEvents.GameEffectManagerEvents.RequestModifyAttribute -= ModifyAttribute;
        GameEvents.GameEffectManagerEvents.RequestGiveGameEffect -= ApplyGameplayEffect;
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
        Debug.Log($"Applied {gameplayEffect.name} to {receiver.name}");
    }
}