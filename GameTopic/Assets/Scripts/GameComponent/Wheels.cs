using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Wheels : MonoBehaviour, ICoreComponent
{
    public Dictionary<string, Ability> AllAbilities { get; private set; }
    public WheelJoint2D wheelJoint;
    private const float MoveForce = 100f;

    private void Awake()
    {
        Debug.Assert(wheelJoint != null, "wheelJoint is null");
        AllAbilities = new Dictionary<string, Ability>
        {
            { "turnRight", new Ability("turnRight", TurnRight) },
            { "turnLeft", new Ability("turnLeft", TurnLeft) }
        };
    }
    private void TurnRight()
    {
        wheelJoint.useMotor = true;
        wheelJoint.motor = new JointMotor2D
        {
            motorSpeed = -MoveForce,
            maxMotorTorque = 10000
        };
    }

    private void TurnLeft()
    {
        wheelJoint.useMotor = true;
        wheelJoint.motor = new JointMotor2D
        {
            motorSpeed = MoveForce,
            maxMotorTorque = 10000
        };
    }
}

