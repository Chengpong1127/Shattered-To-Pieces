using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : BaseCoreComponent, IRotatable {
    [SerializeField] Transform rotateAnchor;

    public Transform RotateBody => BodyTransform;
    public Transform RotateCenter => rotateAnchor;

    protected override void Awake() {
        base.Awake();
    }
}
