using System;
using System.Collections;
using System.Collections.Generic;
using AttributeSystem.Authoring;
using UnityEngine;
using Unity.Netcode;
public class Wheel : BaseCoreComponent,IBodyControlable
{
    [SerializeField]
    private AttributeScriptableObject MoveVelocityAttribute;
    [SerializeField]
    public WheelJoint2D WheelJoint;
    public BaseCoreComponent body { get; private set; }

    protected override void Awake()
    {
        body = this;
        base.Awake();
    }
    [ClientRpc]
    public void WheelJointSetUseMotor_ClientRpc(bool useMotor)
    {
        WheelJoint.useMotor = useMotor;
    }
    [ClientRpc]
    public void WheelJointSetMotor_ClientRpc(float speed, float maxTorque){
        WheelJoint.motor = new JointMotor2D(){
            motorSpeed = speed,
            maxMotorTorque = maxTorque
        };
    }
}

