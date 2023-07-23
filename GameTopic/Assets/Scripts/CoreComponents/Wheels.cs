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
            {"TurnLeft", new Ability("TurnLeft", TurnLeft)},
            {"TurnRight", new Ability("TurnRight", TurnRight)},
            {"Stop", new Ability("Stop", Stop)},
        };
    }
    private void TurnRight()
    {
        wheelJoint.useMotor = true;
        wheelJoint.motor = new JointMotor2D
        {
            motorSpeed = MoveForce,
            maxMotorTorque = 10000
        };
    }

    private void TurnLeft()
    {
        wheelJoint.useMotor = true;
        wheelJoint.motor = new JointMotor2D
        {
            motorSpeed = -MoveForce,
            maxMotorTorque = 10000
        };
    }

    private void Stop(){
        wheelJoint.useMotor = false;
    }
}

