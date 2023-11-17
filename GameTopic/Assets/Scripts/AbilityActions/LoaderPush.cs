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
        List<Collider2D> colliders=new List<Collider2D>() ;
        List<KeyValuePair<FixedJoint2D, float>> forceRecorder = new List<KeyValuePair<FixedJoint2D, float>>();
        Animator entityAnimator;
        IEntityCollisionable entityCollisionable;
        Loader abilityOwner = null;

        bool skillPlaying = false;
        public LoaderPushSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            entityAnimator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            entityCollisionable = (SelfEntity as IEntityCollisionable) ?? throw new System.ArgumentNullException("The entity should have entity triggerable.");
            abilityOwner = SelfEntity as Loader ?? throw new System.ArgumentNullException("The entity should have Loader.");
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
            
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Push"));
            skillPlaying = true;
            SelfEntity.StartCoroutine(ApplyBreakComponent());
            //entityTriggerable.OnTriggerEntity -= TriggerAction;
            // pull Loader back animation
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
            skillPlaying = false;
        }

        IEnumerator ApplyBreakComponent()
        {
            while (skillPlaying)
            {
                colliders.Clear();
                forceRecorder.Clear();
                SelfEntity.BodyColliders[0].OverlapCollider(contactFilter, colliders);
                foreach (var collider in colliders)
                {
                    var et = collider.gameObject.GetComponentInParent<Entity>();
                    if (et == null) { continue; }
                    if (et is BaseCoreComponent coreComponent && (SelfEntity as BaseCoreComponent).HasTheSameRootWith(coreComponent)) continue;

                    et.gameObject.GetComponent<Connector>().BreakConnection();
                }
                yield return new WaitForFixedUpdate();
            }
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

            entityCollisionable.OnCollisionEntity -= TriggerAction;
        }
    }
}
