using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;


[CreateAssetMenu(fileName = "LoaderPush", menuName = "Ability/LoaderPush")]
public class LoaderPush : DisplayableAbilityScriptableObject {
    [SerializeField] float Power;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new LoaderPushSpec(this, owner) {
            Power = Power
        };
        return spec;
    }

    public class LoaderPushSpec : RunnerAbilitySpec {
        public float Power;

        Animator entityAnimator;
        IEntityTriggerable entityTriggerable;
        public LoaderPushSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            entityAnimator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            entityTriggerable = (SelfEntity as IEntityTriggerable) ?? throw new System.ArgumentNullException("The entity should have entity triggerable.");
        }

        public override void CancelAbility() {
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }

        protected override IEnumerator ActivateAbility() {
            entityAnimator.SetTrigger("PushTrigger");
            entityTriggerable.OnTriggerEntity += TriggerAction;
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Push"));
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("PullBack"));
            entityTriggerable.OnTriggerEntity -= TriggerAction;
            // pull Loader back animation
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        }

        protected override IEnumerator PreActivate() {
            yield return null;
        }

        private void TriggerAction(Entity other) {
            var core = other as BaseCoreComponent;
            if(core == null) { return; }
            var rootCore = core.GetRoot() as ICharacterCtrl;
            if(rootCore == null) { return; }

            rootCore.Push(
                (other.BodyColliders[0].transform.position -
                SelfEntity.BodyColliders[0].transform.position)
                .normalized * Power
                );

            entityTriggerable.OnTriggerEntity -= TriggerAction;
        }
    }
}
