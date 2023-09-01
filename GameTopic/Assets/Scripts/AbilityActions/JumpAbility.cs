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

    public class JunmpAbilitySpec : RunnerAbilitySpec {
        public Vector2 Direction;
        public float Power;

        BaseCoreComponent Body;
        ICharacterCtrl Character;
        Animator animator;

        public JunmpAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            var obj = SelfEntity as IBodyControlable ?? throw new System.ArgumentNullException("SelfEntity");
            Body = obj.body;
        }

        public override void CancelAbility() {
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }

        protected override IEnumerator ActivateAbility() {

            if (Character != null && Character.Landing) {
                Character.Move(Body.BodyTransform.TransformDirection(Direction) * Power,ForceMode2D.Impulse);
            }
            yield return null;
        }

        protected override IEnumerator PreActivate() {
            Character = Body.Root as ICharacterCtrl ?? throw new System.ArgumentNullException("Root component need ICharacterCtrl");

            yield return null;
        }
    }
}
