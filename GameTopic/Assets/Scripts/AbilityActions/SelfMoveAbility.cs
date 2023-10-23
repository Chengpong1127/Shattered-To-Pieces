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
        public IMovable Movable => SelfEntity as IMovable;
        public float Speed;
        public MoveDirection Direction;
        public string AnimationName;
        public float EnergyCostPerSecond;
        private float currentSpeed;
        public MoveAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
        }

        public override void CancelAbility()
        {
            if (AnimationName != "") SelfEntity.BodyAnimator?.SetBool(AnimationName, false);
            Movable.StopMove(currentSpeed);
            currentSpeed = 0;
            EndAbility();
        }

        protected override IEnumerator ActivateAbility()
        {
            currentSpeed = 0;
            if (SelfEntity is IGroundCheckable groundCheckable)
            {
                if (!groundCheckable.IsGrounded) yield break;
            }
            if (AnimationName != "") SelfEntity.BodyAnimator?.SetBool(AnimationName, true);
            switch (Direction)
            {
                case MoveDirection.Left:
                    currentSpeed = -Speed;
                    break;
                case MoveDirection.Right:
                    currentSpeed = Speed;
                    break;
            }
            Movable.StartMove(currentSpeed);
            while(isActive){
                if (EnergyManager.HasEnergy(EnergyCostPerSecond * Time.deltaTime * 2))
                    EnergyManager.CostEnergy(EnergyCostPerSecond * Time.deltaTime);
                else{
                    CancelAbility();
                    yield break;
                }
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