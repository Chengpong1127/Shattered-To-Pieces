using System;
using UnityEngine;  
using AbilitySystem.Authoring;

public class GameComponentAbility{
    /// <summary>
    /// The index of the ability in a core component.
    /// </summary>
    public int AbilityIndex;

    /// <summary>
    /// The game component that own this ability.
    /// </summary>
    /// <value></value>
    public readonly ICoreComponent OwnerGameComponent;

    public AbstractAbilityScriptableObject AbilityScriptableObject;
    public AbstractAbilitySpec AbilitySpec;

    public GameComponentAbility(int index, ICoreComponent owner, AbstractAbilityScriptableObject abilityScriptableObject, AbstractAbilitySpec abilitySpec){
        AbilityIndex = index;
        OwnerGameComponent = owner;
        AbilityScriptableObject = abilityScriptableObject;
        AbilitySpec = abilitySpec;
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
