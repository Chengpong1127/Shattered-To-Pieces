using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using DG.Tweening;
[CreateAssetMenu(fileName = "RubbergunAbility", menuName = "Ability/RubbergunAbility")]
public class RubbergunAbility : AbstractAbilityScriptableObject
{
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {

        var spec = new RubbergunAbilitySpec(this, owner)
        {
        };

        return spec;

    }
    protected class RubbergunAbilitySpec : RunnerAbilitySpec
    {

        public RubbergunAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
        }
        public override void CancelAbility()
        {
            return;
        }

        public override bool CheckGameplayTags()
        {
            return true;
        }

        protected override IEnumerator ActivateAbility()
        {
            yield return null;
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}

