using System.Collections.Generic;
using AbilitySystem.Authoring;
using UnityEngine;
using System.Linq;

public abstract class AbilityEntity: Entity{
    [SerializeField]
    protected AbstractAbilityScriptableObject[] Abilities;
    [SerializeField]
    protected AbstractAbilityScriptableObject[] InitializationAbilities;
    private readonly Dictionary<AbstractAbilityScriptableObject, AbstractAbilitySpec> _abilityMap = new();
    protected override void Awake(){
        base.Awake();
        Abilities = Abilities.Where(x => x != null).ToArray();
        InitializationAbilities = InitializationAbilities.Where(x => x != null).ToArray();
        ActivateInitializationAbilities();
        GrantCastableAbilities();
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
    private void GrantCastableAbilities()
    {
        foreach (var ability in Abilities)
        {
            var spec = ability.CreateSpec(AbilitySystemCharacter);
            AbilitySystemCharacter.GrantAbility(spec);
            _abilityMap.Add(ability, spec);
        }
    }

    public AbstractAbilityScriptableObject[] GetAbilities()
    {
        return Abilities;
    }
    public AbstractAbilitySpec[] GetAbilitySpecs()
    {
        return _abilityMap.Values.ToArray();
    }
}