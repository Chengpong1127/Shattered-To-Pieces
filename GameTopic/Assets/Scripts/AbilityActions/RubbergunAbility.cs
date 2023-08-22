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
    protected class RubbergunAbilitySpec : EntityAbilitySpec
    {
        public float DurationTime;
        public GameObject g_light;
        public bool clear;
        public RubbergunAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            g_light = SelfEntity.GetComponent<LightScript>().g_light;
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

