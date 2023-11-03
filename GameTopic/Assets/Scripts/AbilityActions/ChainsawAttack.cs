using System.Linq;
using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChainsawAttack", menuName = "Ability/ChainsawAttack")]
public class ChainsawAttack : DisplayableAbilityScriptableObject {
    public GameplayEffectScriptableObject BatDamageEffect;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new ChainsawAttackSpec(this, owner) {
            DamageEffect = BatDamageEffect
        };
        return spec;
    }

    public class ChainsawAttackSpec : RunnerAbilitySpec {
        public GameplayEffectScriptableObject DamageEffect;

        Animator entityAnimator;
        IEntityCollisionable entityTriggerable;
        public ChainsawAttackSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            entityAnimator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            entityTriggerable = (SelfEntity as IEntityCollisionable) ?? throw new System.ArgumentNullException("The entity should have entity triggerable.");
        }

        public override void CancelAbility() {
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }

        protected override IEnumerator ActivateAbility() {
            entityAnimator.SetTrigger("ATKTrigger");
            entityTriggerable.OnCollisionEntity += TriggerAction;
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
            yield return new WaitUntil(() => !entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
            entityTriggerable.OnCollisionEntity -= TriggerAction;
        }

        private void TriggerAction(Entity other) {
            GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(SelfEntity, other, DamageEffect);
            entityTriggerable.OnCollisionEntity -= TriggerAction;
        }
    }
}
