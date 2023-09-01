using AbilitySystem;
using AbilitySystem.Authoring;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpAbility", menuName = "Ability/JumpAbility")]
public class JumpAbility : AbstractAbilityScriptableObject {

    [SerializeField] Vector3 Direction;
    [SerializeField] float Power;


    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new JunmpAbilitySpec(this, owner) {
            Direction = Direction,
            Power = Power
        };
        return spec;
    }

    public class JunmpAbilitySpec : EntityAbilitySpec {
        public Vector2 Direction;
        public float Power;

        BaseCoreComponent Body;
        CharacterController Character;
        Animator animator;

        static ContactFilter2D filter = new();
        List<Collider2D> collisionResult = new();
        bool landing;

        public JunmpAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            var obj = SelfEntity as IBodyControlable ?? throw new System.ArgumentNullException("SelfEntity");
            Body = obj.body;

            filter.useTriggers = true;
            filter.useLayerMask = false;
        }

        public override void CancelAbility() {
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }

        protected override IEnumerator ActivateAbility() {

            if (Body.BodyCollider.OverlapCollider(filter, collisionResult) != 0) {
                collisionResult.ForEach(collider => {
                    var obj = collider.gameObject.GetComponent<BaseCoreComponent>();
                    if(obj == null || !obj.HasTheSameRootWith(Body)) { landing = true; }
                });
                if (landing && Character != null) {
                    // Body.Root.BodyRigidbody.AddForce(
                    //     Body.BodyTransform.TransformDirection(Direction) * Power,
                    //     ForceMode2D.Impulse
                    // );
                    Character.Move(Body.BodyTransform.TransformDirection(Direction) * Power * Time.fixedDeltaTime);
                }
            }

            yield return null;
        }

        protected override IEnumerator PreActivate() {
            Character = (Body.Root as ICharacter)?.Character ?? throw new System.ArgumentNullException("Root entity has no CharacterController.");

            landing = false;
            yield return null;
        }
    }
}
