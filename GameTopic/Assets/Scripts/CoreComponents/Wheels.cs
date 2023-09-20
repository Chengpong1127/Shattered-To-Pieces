using System;
using System.Collections;
using System.Collections.Generic;
using AttributeSystem.Authoring;
using UnityEngine;
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
        MoveDecorator.Instance.Decorate(this);
    }
}

