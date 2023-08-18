using System.Collections.Generic;
using AbilitySystem.Authoring;
using UnityEngine;
using System.Linq;

/// <summary>
/// AbilityEntity is an entity that has main abilities.
/// </summary>
public class AbilityEntity: Entity{
    [SerializeField]
    protected AbstractAbilityScriptableObject[] Abilities;
    private readonly Dictionary<AbstractAbilityScriptableObject, AbstractAbilitySpec> _abilityMap = new();
    protected override void Awake(){
        base.Awake();
        Abilities = Abilities.Where(x => x != null).ToArray();
        InitializationAbilities = InitializationAbilities.Where(x => x != null).ToArray();
        GrantCastableAbilities();
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