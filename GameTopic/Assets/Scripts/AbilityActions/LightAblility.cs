using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using DG.Tweening;
[CreateAssetMenu(fileName = "LightAbility", menuName = "Ability/LightAbility")]
public class LightAbility : AbstractAbilityScriptableObject
{
    protected float DurationTime = 4.0f;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {

        var spec = new LightAbilitySpec(this, owner)
        {
            DurationTime = DurationTime,
        };

        return spec;

    }
    protected class LightAbilitySpec : EntityAbilitySpec
    {
        public float DurationTime;
        public GameObject g_light;
        public bool clear;
        public LightAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
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
            g_light.GetComponent<SpriteRenderer>().enabled=true;
            g_light.GetComponent<Collider2D>().enabled = true;
            yield return new WaitForSeconds(DurationTime);
            g_light.GetComponent<SpriteRenderer>().enabled = false;
            g_light.GetComponent<Collider2D>().enabled = false;
            var clear = SelfEntity.GetComponent<LightScript>().clear;
            clear = true;
            yield return null;
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}

