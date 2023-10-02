using AbilitySystem;
using AbilitySystem.Authoring;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpAbility", menuName = "Ability/JumpAbility")]
public class JumpAbility : DisplayableAbilityScriptableObject {

    [SerializeField] float Power;
    [SerializeField] float JumpTime;
    [SerializeField] float JumpMultipiler;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new JunmpAbilitySpec(this, owner) {
            Power = Power,
            JumpTime = JumpTime,
            JumpMultipiler = JumpMultipiler
        };
        return spec;
    }

    public class JunmpAbilitySpec : RunnerAbilitySpec {
        public float Power;
        public float JumpTime;
        public float JumpMultipiler;
        BaseCoreComponent Body;
        ICharacterCtrl Character;
        Animator animator;
        float JumpCounter;
        bool isJumping;
        public JunmpAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            var obj = SelfEntity as IBodyControlable ?? throw new System.ArgumentNullException("SelfEntity");
            Body = obj.body;
        }

        public override void CancelAbility() {
            isJumping = false;
            if (SelfEntity.BodyRigidbody.velocity.y > 0)
            {
                SelfEntity.BodyRigidbody.velocity = new Vector2(SelfEntity.BodyRigidbody.velocity.x, SelfEntity.BodyRigidbody.velocity.y * 0.6f);
            }
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }

        protected override IEnumerator ActivateAbility() {

            if (SelfEntity is IGroundCheckable groundCheckable)
            {
                if (!groundCheckable.IsGrounded) yield break;
            }
            // Character.Move(Body.BodyTransform.TransformDirection(Direction) * Power);
            // Character.VerticalMove(Power);
            SelfEntity.BodyRigidbody.velocity = new Vector2(SelfEntity.BodyRigidbody.velocity.x, Power);
            while (isJumping && SelfEntity.BodyRigidbody.velocity.y > 0)
            {
                JumpCounter += Time.deltaTime;
                if (JumpCounter > JumpTime) isJumping = false;
                float time = JumpCounter / JumpTime;
                float currentJumpM = JumpMultipiler;
                if (time < 0.5f)
                {
                    currentJumpM = JumpMultipiler * (1 - time);
                }
                new Vector2(0, -Physics2D.gravity.y);
                SelfEntity.BodyRigidbody.velocity += new Vector2(0, -Physics2D.gravity.y) * currentJumpM * Time.deltaTime;
                yield return null;
            }
            //Character.AddForce(Body.BodyTransform.TransformDirection(Vector3.up) * Power, ForceMode2D.Impulse);  
            yield return null;
        }

        protected override IEnumerator PreActivate() {
           // Character = Body.GetRoot() as ICharacterCtrl ?? throw new System.ArgumentNullException("Root component need ICharacterCtrl");
            JumpCounter = 0;
            isJumping = true;
            yield return null;
        }
    }
}
