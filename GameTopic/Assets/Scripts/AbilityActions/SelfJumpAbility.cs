using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using AbilitySystem.ModifierMagnitude;


[CreateAssetMenu(fileName = "SelfJumpAbility", menuName = "Ability/SelfJumpAbility")]
public class SelfJumpAbility : DisplayableAbilityScriptableObject
{
    public float Power;
    public string AnimationName;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new JumpAbilitySpec(this, owner)
        {
            Power = Power,
            AnimationName = AnimationName
        };
        return spec;
    }

    public class JumpAbilitySpec : RunnerAbilitySpec
    {
        public float Power;
        public MoveDirection Direction;
        public string AnimationName;
        public JumpAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
        }

        public override void CancelAbility()
        {
            if (AnimationName != "") SelfEntity.BodyAnimator?.SetBool(AnimationName, false);
            EndAbility();
        }

        protected override IEnumerator ActivateAbility()
        {
            if (SelfEntity is IGroundCheckable groundCheckable)
            {
                if (!groundCheckable.IsGrounded) yield break;
            }
            if (AnimationName != "") SelfEntity.BodyAnimator?.SetBool(AnimationName, true);
            var forceAddable = SelfEntity as IForceAddable;
            forceAddable.AddForce(SelfEntity.BodyTransform.up * Power, ForceMode2D.Impulse);
            yield return null;
        }
    }
    public enum MoveDirection
    {
        Left,
        Right,
    }
}