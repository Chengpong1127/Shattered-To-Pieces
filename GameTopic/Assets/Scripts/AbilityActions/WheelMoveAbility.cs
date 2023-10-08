using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using AbilitySystem.ModifierMagnitude;


[CreateAssetMenu(fileName = "WheelMoveAbility", menuName = "Ability/WheelMoveAbility")]
public class WheelMoveAbility : DisplayableAbilityScriptableObject
{
    public float Speed;
    public MoveDirection Direction;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new WheelMoveAbilitySpec(this, owner)
        {
            Speed = Speed,
            Direction = Direction
        };
        return spec;
    }

    public class WheelMoveAbilitySpec : RunnerAbilitySpec
    {
        public float Speed;
        public MoveDirection Direction;
        public Wheel wheel;
        public WheelMoveAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            wheel = SelfEntity as Wheel;
        }

        public override void CancelAbility()
        {
            wheel.WheelJointSetUseMotor_ClientRpc(false);
            EndAbility();
        }

        protected override IEnumerator ActivateAbility()
        {
            wheel.WheelJointSetMotor_ClientRpc(Direction == MoveDirection.Left ? -Speed : Speed, 10000);
            wheel.WheelJointSetUseMotor_ClientRpc(true);
            yield return null;
        }
    }
    public enum MoveDirection
    {
        Left,
        Right,
    }
}