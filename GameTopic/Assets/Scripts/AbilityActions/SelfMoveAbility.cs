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
        public MoveAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
        }

        public override void CancelAbility()
        {
            Movable.StopMoveDirection(Direction);
            EndAbility();
        }

        protected override IEnumerator ActivateAbility()
        {
            Movable.SetMovingSpeed(Speed);
            if (SelfEntity is IGroundCheckable groundCheckable)
            {
                if (!groundCheckable.IsGrounded) yield break;
            }
            Movable.SetMoveDirection(Direction);
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
}