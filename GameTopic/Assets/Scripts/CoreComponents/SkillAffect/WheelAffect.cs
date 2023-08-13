using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WheelAffect", menuName = "SkillAffect/WheelAffect")]
public class WheelAffect : SkillAffectBase
{
    public WheelJoint2D wheelJoint { get; set; } = null;
    public bool direction { get; set; } //True=Right;False=Left
    const float MoveForce = 200f;
    public WheelAffect(){
        this.type = SkillAffectType.Move;
    }
    public void InvokeStart()
    {
        if (!execute && interrupt) return;
        wheelJoint.useMotor = true;
        interrupt = false;
        execute = true;
    }

    override public void Invoke()
    {
        if (!execute && interrupt) return;
        wheelJoint.motor = new JointMotor2D
        {
            motorSpeed = (direction?1f:-1f)*MoveForce,
            maxMotorTorque = 10000
        };
    }

    public void End()
    {
        wheelJoint.useMotor = false;
    }

}
