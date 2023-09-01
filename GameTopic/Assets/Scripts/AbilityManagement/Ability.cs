using System;
using UnityEngine;  
using AbilitySystem.Authoring;

public class GameComponentAbility{
    /// <summary>
    /// The index of the ability in a core component.
    /// </summary>
    public int AbilityIndex;
    public string AbilityName => AbilityScriptableObject.AbilityName;

    /// <summary>
    /// The game component that own this ability.
    /// </summary>
    /// <value></value>
    public readonly ICoreComponent OwnerGameComponent;

    public AbstractAbilityScriptableObject AbilityScriptableObject;
    public RunnerAbilitySpec AbilitySpec;

    public GameComponentAbility(int index, ICoreComponent owner, AbstractAbilityScriptableObject abilityScriptableObject, RunnerAbilitySpec abilitySpec){
        AbilityIndex = index;
        OwnerGameComponent = owner ?? throw new ArgumentNullException(nameof(owner));
        AbilityScriptableObject = abilityScriptableObject ?? throw new ArgumentNullException(nameof(abilityScriptableObject));
        AbilitySpec = abilitySpec ?? throw new ArgumentNullException(nameof(abilitySpec));
    }


    public override bool Equals(object obj)
    {
        return obj is GameComponentAbility ability 
            && AbilityIndex == ability.AbilityIndex 
            && OwnerGameComponent == ability.OwnerGameComponent
            && AbilityScriptableObject == ability.AbilityScriptableObject
            && AbilitySpec == ability.AbilitySpec;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(AbilityIndex, OwnerGameComponent, AbilityScriptableObject, AbilitySpec);
    }
}
