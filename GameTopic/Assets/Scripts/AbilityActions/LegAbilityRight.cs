using AbilitySystem;
using AbilitySystem.Authoring;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

[CreateAssetMenu(fileName = "LegAbilityRight", menuName = "Ability/LegAbilityRight")]
public class LegAbilityRight : AbstractAbilityScriptableObject {
    [SerializeField] Vector3 Direction;
    [SerializeField] float Speed;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new LegAbilityRightSpec(this, owner) {
            Direction = Direction,
            Speed = Speed
        };

        return spec;
    }

    public class LegAbilityRightSpec : EntityAbilitySpec {
        public Vector3 Direction;
        public float Speed;
        bool Active;

        BaseCoreComponent Body;
        Animator animator;

        static ContactFilter2D filter = new();
        List<Collider2D> collisionResult = new();
        bool landing;

        public LegAbilityRightSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            var obj = SelfEntity as IBodyControlable ?? throw new System.ArgumentNullException("SelfEntity");
            Body = obj.body;
            Active = false;

            filter.useTriggers = true;
            filter.useLayerMask = false;
        }
        public override void CancelAbility() {
            Active = false;
            animator.SetBool("Move", false);
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }
        protected override IEnumerator ActivateAbility() {

            if (Body.BodyCollider.OverlapCollider(filter, collisionResult) != 0) {
                collisionResult.ForEach(collider => {
                    var obj = collider.gameObject.GetComponent<BaseCoreComponent>();
                    if (obj == null || !obj.HasTheSameRootWith(Body)) { landing = true; }
                });
            }

            if(Active && landing) {
                animator.SetBool("Move", true);
            }

            while (Active && landing) {
                Body.Root.BodyRigidbody.AddForce(
                    Body.BodyTransform.TransformDirection(Direction) * Speed
                );

                landing = false;
                if (Body.BodyCollider.OverlapCollider(filter, collisionResult) != 0) {
                    collisionResult.ForEach(collider => {
                        var obj = collider.gameObject.GetComponent<BaseCoreComponent>();
                        if (obj == null || !obj.HasTheSameRootWith(Body)) { landing = true; }
                    });
                }
                yield return null;
            }

            animator.SetBool("Move", false);
            yield return null;
        }
        protected override IEnumerator PreActivate() {
            Active = true;
            landing = false;
            animator.SetFloat("Speed", Speed);
            yield return null;
        }
    }
}
