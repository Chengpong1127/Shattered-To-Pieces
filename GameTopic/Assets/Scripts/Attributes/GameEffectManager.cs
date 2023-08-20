using AbilitySystem;
using AttributeSystem.Components;
using AttributeSystem.Authoring;
using AbilitySystem.Authoring;
using UnityEngine;
using System;
public class GameEffectManager{
    public GameEffectManager(){
        this.StartListening(EventName.GameEffectManagerEvents.RequestModifyAttribute, new Action<Entity, Entity, GameplayEffectModifier>(ModifyAttribute));
        this.StartListening(EventName.GameEffectManagerEvents.RequestGiveGameEffect, new Action<Entity, Entity, GameplayEffectScriptableObject>(ApplyGameplayEffect));
    }
    private void ModifyAttribute(Entity sender, Entity receiver, GameplayEffectModifier modifier){
        var effect = ScriptableObject.CreateInstance<GameplayEffectScriptableObject>();
        effect.gameplayEffect.Modifiers = new GameplayEffectModifier[]{modifier};
        ApplyGameplayEffect(sender, receiver, effect);
    }
    private void ApplyGameplayEffect(Entity sender, Entity receiver, GameplayEffectScriptableObject gameplayEffect){
        var spec = sender.AbilitySystemCharacter.MakeOutgoingSpec(gameplayEffect);
        receiver.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(spec);
    }
}