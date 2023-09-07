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
    public float RegenerationTime;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new RegenerationAbilitySpec(this, owner)
        {
            RegenerationEffect = RegenerationEffect,
            RegenerationTime = RegenerationTime
        };
        return spec;
    }
    public class RegenerationAbilitySpec : RunnerAbilitySpec
    {
        public GameplayEffectScriptableObject RegenerationEffect;
        public float RegenerationTime;
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
            var component = LocalPlayerInputManager.Instance.GetGameObjectUnderMouse()?.GetComponentInParent<GameComponent>();
            if (component == null) yield break;
            var baseCoreComponent = component.CoreComponent;
            if (baseCoreComponent == null) yield break;
            Debug.Log("Start Regeneration");
            
            yield return new WaitForSeconds(RegenerationTime);
            GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(SelfEntity, baseCoreComponent, RegenerationEffect);
        }

        protected override IEnumerator PreActivate()
        {
            yield return base.PreActivate();
        }
    }
}