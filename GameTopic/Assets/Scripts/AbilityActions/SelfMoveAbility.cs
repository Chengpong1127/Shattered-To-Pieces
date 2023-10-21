using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using AbilitySystem.ModifierMagnitude;
using Unity.Netcode;


[CreateAssetMenu(fileName = "MoveAbility", menuName = "Ability/MoveAbility")]
public class SelfMoveAbility : DisplayableAbilityScriptableObject
{
    public float Speed;
    public MoveDirection Direction;
    public string AnimationName;
    public float EnergyCostPerSecond;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new MoveAbilitySpec(this, owner)
        {
            Speed = Speed,
            Direction = Direction,
            AnimationName = AnimationName,
            EnergyCostPerSecond = EnergyCostPerSecond,
        };
        return spec;
    }

    public class MoveAbilitySpec : RunnerAbilitySpec
    {
        public float Speed;
        public MoveDirection Direction;
        public string AnimationName;
        public float EnergyCostPerSecond;
        public MoveAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
        }

        public override void CancelAbility()
        {
            var velocity = SelfEntity.BodyRigidbody.velocity;
            velocity.x = 0;
            SelfEntity.BodyRigibodySetVelocity_ClientRpc(velocity);
            if (AnimationName != "") SelfEntity.BodyAnimator?.SetBool(AnimationName, false);
            EndAbility();
        }

        protected override IEnumerator ActivateAbility()
        {
            if (SelfEntity is IGroundCheckable groundCheckable)
            {
                if (!groundCheckable.IsGrounded) yield break;
            }
            while(isActive){
                if (EnergyManager.HasEnergy(EnergyCostPerSecond * Time.fixedDeltaTime * 2))
                    EnergyManager.CostEnergy(EnergyCostPerSecond * Time.fixedDeltaTime);
                else{
                    CancelAbility();
                    yield break;
                }
                if (AnimationName != "") SelfEntity.BodyAnimator?.SetBool(AnimationName, true);
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
                SelfEntity.BodyRigibodySetVelocity_ClientRpc(velocity);
                yield return new WaitForFixedUpdate();
            }
        }
    }
    public enum MoveDirection
    {
        Left,
        Right,
    }
}