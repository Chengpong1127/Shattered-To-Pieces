using AttributeSystem.Components;
using AbilitySystem;
using UnityEngine;
using AbilitySystem.Authoring;
using System;
using System.Collections;
using System.Linq;

/// <summary>
/// Entity is the base class for all entities in the game. An entity has attributes and the init abilities to set up the init state of the entity.
/// </summary>
[RequireComponent(typeof(AttributeSystemComponent)), RequireComponent(typeof(AbilitySystemCharacter))]
public class Entity: BaseEntity{
    [HideInInspector]
    public AttributeSystemComponent AttributeSystemComponent;
    [HideInInspector]
    public AbilitySystemCharacter AbilitySystemCharacter;
    public bool IsInitialized { get; private set;}
    [SerializeField]
    protected AbstractAbilityScriptableObject[] InitializationAbilities;
    protected override void Awake() {
        base.Awake();
        AttributeSystemComponent ??= GetComponent<AttributeSystemComponent>();
        AbilitySystemCharacter ??= GetComponent<AbilitySystemCharacter>();
        AbilitySystemCharacter.AttributeSystem = AttributeSystemComponent;
        StartCoroutine(ActivateInitializationAbilities());
    }
    private IEnumerator ActivateInitializationAbilities()
    {
        AbstractAbilitySpec[] specs = new AbstractAbilitySpec[InitializationAbilities.Length];
        for (int i = 0; i < InitializationAbilities.Length; i++)
        {
            if (InitializationAbilities[i] == null) continue;
            specs[i] = InitializationAbilities[i].CreateSpec(AbilitySystemCharacter);
            AbilitySystemCharacter.GrantAbility(specs[i]);
            StartCoroutine(specs[i].TryActivateAbility());
        }
        yield return new WaitUntil(() => specs.All(spec => !spec.isActive));
        IsInitialized = true;
    }
}