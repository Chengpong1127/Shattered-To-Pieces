using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using AbilitySystem.ModifierMagnitude;

[CreateAssetMenu(fileName = "RegenerationAbility", menuName = "Ability/RegenerationAbility")]
public class RegenerationAbility : DisplayableAbilityScriptableObject
{
    public GameplayEffectScriptableObject RegenerationEffect;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new RegenerationAbilitySpec(this, owner)
        {
            RegenerationEffect = RegenerationEffect
        };
        return spec;
    }
    public class RegenerationAbilitySpec : RunnerAbilitySpec
    {
        public GameplayEffectScriptableObject RegenerationEffect;
        public RegenerationAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
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
            var component = Utils.GetGameObjectUnderMouse()?.GetComponentInParent<GameComponent>();
            if (component == null) yield break;
            var baseCoreComponent = component.CoreComponent;
            if (baseCoreComponent == null) yield break;
            Debug.Log("Start Regeneration");
            GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(SelfEntity, baseCoreComponent, RegenerationEffect);
        }

        protected override IEnumerator PreActivate()
        {
            yield return base.PreActivate();
        }
    }
}