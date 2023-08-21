using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : BaseCoreComponent, IRotatable {
    [SerializeField] Transform rotateAnchor;

    public Transform RotateBody => rotateAnchor;
    public Transform RotateCenter => rotateAnchor;

    protected override void Awake() {
        base.Awake();
    }
}