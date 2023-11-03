using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using AbilitySystem.ModifierMagnitude;
using System.Linq;
[CreateAssetMenu(fileName = "BatRotation", menuName = "Ability/BatRotation")]
public class BatRotation : DisplayableAbilityScriptableObject
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
        private IEntityCollisionable entityTriggerable;
        public GameplayEffectScriptableObject DamageEffect;

        public BatRotationSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            entityAnimator = (SelfEntity as BaseCoreComponent)?.BodyAnimator;
            entityTriggerable = (SelfEntity as IEntityCollisionable) ?? throw new System.ArgumentNullException("The entity should have entity triggerable.");
        }

        public override void CancelAbility()
        {
            return;
        }

        protected override IEnumerator ActivateAbility()
        {
            entityAnimator.Play("Swing");
            entityTriggerable.OnCollisionEntity += TriggerAction;
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Swing"));
            yield return new WaitUntil(() => !entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Swing"));
            entityTriggerable.OnCollisionEntity -= TriggerAction;
        }

        private void TriggerAction(Entity other){
            entityTriggerable.OnCollisionEntity -= TriggerAction;
            GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(SelfEntity, other, DamageEffect);
        }
    }
}