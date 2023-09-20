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
        public WheelJoint2D WheelJoint2D;
        public WheelMoveAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            WheelJoint2D = (SelfEntity as Wheel)?.WheelJoint;
            Debug.Assert(WheelJoint2D != null, "The entity should have WheelJoint2D.");
        }

        public override void CancelAbility()
        {
            WheelJoint2D.useMotor = false;
            EndAbility();
        }

        protected override IEnumerator ActivateAbility()
        {
            WheelJoint2D.motor = new JointMotor2D() {
                motorSpeed = Direction == MoveDirection.Left ? -Speed : Speed,
                maxMotorTorque = 10000 
            };
            WheelJoint2D.useMotor = true;
            yield return null;
        }
    }
    public enum MoveDirection
    {
        Left,
        Right,
    }
}