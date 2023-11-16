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
        ContactFilter2D contactFilter;
        List<Collider2D> colliders;
        Animator entityAnimator;
        IEntityCollisionable entityCollisionable;
        public LoaderPushSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            entityAnimator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            entityCollisionable = (SelfEntity as IEntityCollisionable) ?? throw new System.ArgumentNullException("The entity should have entity triggerable.");
        }

        public override void CancelAbility() {
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }

        protected override IEnumerator ActivateAbility() {
            entityAnimator.SetTrigger("PushTrigger");
            //entityTriggerable.OnTriggerEntity += TriggerAction;
            colliders.Clear();
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Push"));
            SelfEntity.BodyColliders[0].OverlapCollider(contactFilter, colliders);
            foreach (var collider in colliders) {
                var et = collider.gameObject.GetComponentInParent<Entity>();
                if(et == null) { continue; }
                if (et is BaseCoreComponent coreComponent && (SelfEntity as BaseCoreComponent).HasTheSameRootWith(coreComponent)) continue;
                et.gameObject.GetComponent<Connector>().Disconnect();
            }
            //entityTriggerable.OnTriggerEntity -= TriggerAction;
            // pull Loader back animation
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
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

            entityCollisionable.OnCollisionEntity -= TriggerAction;
        }
    }
}
