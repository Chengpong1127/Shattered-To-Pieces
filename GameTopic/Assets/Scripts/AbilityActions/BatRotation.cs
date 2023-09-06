using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using AbilitySystem.ModifierMagnitude;
[CreateAssetMenu(fileName = "BatRotation", menuName = "Ability/BatRotation")]
public class BatRotation : AbstractAbilityScriptableObject
{
    public GameplayEffectScriptableObject BatDamageEffect;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new BatRotationSpec(this, owner)
        {
            DamageEffect = BatDamageEffect
        };
        return spec;
    }

    public class BatRotationSpec : RunnerAbilitySpec
    {
        private Animator entityAnimator;
        private IEntityTriggerable entityTriggerable;
        public GameplayEffectScriptableObject DamageEffect;

        public BatRotationSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            entityAnimator = (SelfEntity as BaseCoreComponent)?.BodyAnimator;
            entityTriggerable = (SelfEntity as IEntityTriggerable) ?? throw new System.ArgumentNullException("The entity should have entity triggerable.");
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
            entityAnimator.Play("Swing");
            entityTriggerable.OnTriggerEntity += TriggerAction;
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Swing"));
            yield return new WaitUntil(() => !entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Swing"));
            entityTriggerable.OnTriggerEntity -= TriggerAction;
        }

        private void TriggerAction(Entity other){
            GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(SelfEntity, other, DamageEffect);
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}