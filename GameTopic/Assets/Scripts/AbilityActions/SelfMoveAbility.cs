using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using AbilitySystem.ModifierMagnitude;


[CreateAssetMenu(fileName = "MoveAbility", menuName = "Ability/MoveAbility")]
public class SelfMoveAbility : DisplayableAbilityScriptableObject
{
    public float Speed;
    public MoveDirection Direction;
    public string AnimationName;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new MoveAbilitySpec(this, owner)
        {
            Speed = Speed,
            Direction = Direction,
            AnimationName = AnimationName
        };
        return spec;
    }

    public class MoveAbilitySpec : RunnerAbilitySpec
    {
        public float Speed;
        public MoveDirection Direction;
        public string AnimationName;
        public MoveAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
        }

        public override void CancelAbility()
        {
            var velocity = SelfEntity.BodyRigidbody.velocity;
            velocity.x = 0;
            SelfEntity.BodyRigidbody.velocity = velocity;
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
            while(isActive){
                var velocity = SelfEntity.BodyRigidbody.velocity;
                switch (Direction)
                {
                    case MoveDirection.Left:
                        velocity.x = -Speed;
                        break;
                    case MoveDirection.Right:
                        velocity.x = Speed;
                        break;
                }
                SelfEntity.BodyRigidbody.velocity = velocity;
                yield return null;
            }
        }
    }
    public enum MoveDirection
    {
        Left,
        Right,
    }
}