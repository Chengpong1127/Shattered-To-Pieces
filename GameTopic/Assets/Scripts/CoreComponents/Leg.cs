using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class Leg : BaseCoreComponent , IBodyControlable, IGroundCheckable {
    public BaseCoreComponent body { get; private set; }

    public bool IsGrounded => throw new System.NotImplementedException();

    protected override void Awake() {
        body = this;
        base.Awake();
    }
}
