using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flour : BaseCoreComponent, IAffectObjectOwner {
    public IAffectedObject affectedObject { get { return affectedObjectInstance; } }
    public AffectedObjectBase affectedObjectInstance { get; set; }

    ThrowOutAffect affect { get;set; }

    protected override void Awake() {
        base.Awake();
        affect = ScriptableObject.CreateInstance<ThrowOutAffect>();
        affectedObjectInstance = ScriptableObject.CreateInstance<AffectedObjectBase>();

        affectedObject.collider = GetComponentInParent<Collider2D>();
        affectedObject.rigidbody = GetComponentInParent<Rigidbody2D>();
        affectedObject.joint = GetComponentInParent<AnchoredJoint2D>();
        affectedObject.transform = gameObject.transform;
        affectedObject.coreComponent = this;

        affect.owner = affectedObject;
        affect.affectedObjectList.Add(affectedObject);

        // AllAbilities.Add("ThrowFlour", new Ability("ThrowFlour", AbiliityStart, AbilityRunning, AbiliityEnd, this));
    }

    public void AbiliityStart() {
        affect.InvokeStart();
        StartCoroutine(affect.FrameRunner());
    }
    public void AbilityRunning() {
        // affect.Invoke();
    }
    public void AbiliityEnd() {
        // affect.SetToDefault();
    }
}
