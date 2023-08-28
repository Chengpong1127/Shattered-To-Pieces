using System;
using System.Collections;
using System.Collections.Generic;
using AttributeSystem.Authoring;
using UnityEngine;
public class Wheel : BaseCoreComponent
{
    [SerializeField]
    private AttributeScriptableObject MoveVelocityAttribute;

    protected override void Awake()
    {
        base.Awake();
        MoveDecorator.Instance.Decorate(this);
    }
}

