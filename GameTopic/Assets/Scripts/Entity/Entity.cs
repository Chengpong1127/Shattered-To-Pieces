using AttributeSystem.Components;
using AbilitySystem;
using UnityEngine;
using AbilitySystem.Authoring;

/// <summary>
/// Entity is the base class for all entities in the game. An entity has attributes and the init abilities to set up the init state of the entity.
/// </summary>
[RequireComponent(typeof(AttributeSystemComponent)), RequireComponent(typeof(AbilitySystemCharacter))]
public class Entity: BaseEntity{
    public AttributeSystemComponent AttributeSystemComponent;
    public AbilitySystemCharacter AbilitySystemCharacter;
    [SerializeField]
    protected AbstractAbilityScriptableObject[] InitializationAbilities;
    protected override void Awake() {
        base.Awake();
        AttributeSystemComponent ??= GetComponent<AttributeSystemComponent>();
        AbilitySystemCharacter ??= GetComponent<AbilitySystemCharacter>();
        AbilitySystemCharacter.AttributeSystem = AttributeSystemComponent;
        ActivateInitializationAbilities();
    }
    private void ActivateInitializationAbilities()
    {
        foreach (var initializationAbility in InitializationAbilities)
        {
            var spec = initializationAbility.CreateSpec(AbilitySystemCharacter);
            AbilitySystemCharacter.GrantAbility(spec);
            StartCoroutine(spec.TryActivateAbility());
        }
    }
}