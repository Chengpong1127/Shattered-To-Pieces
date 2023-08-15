using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : BaseCoreComponent, IAffectObjectOwner {
    public IAffectedObject affectedObject { get { return affectedObjectInstance; } }
    public AffectedObjectBase affectedObjectInstance { get; set; }
    RotatorAffect affect { get; set; }
    [field: SerializeField] Transform connectionAnchor { get;set; }

    private void Awake() {
        affect = ScriptableObject.CreateInstance<RotatorAffect>();
        affectedObjectInstance = ScriptableObject.CreateInstance<AffectedObjectBase>();

        affectedObject.collider = GetComponentInParent<Collider2D>();
        affectedObject.rigidbody = GetComponentInParent<Rigidbody2D>();
        affectedObject.joint = GetComponentInParent<AnchoredJoint2D>();
        affectedObject.transform = gameObject.transform;

        affect.owner = affectedObject;
        affect.affectedObjectList.Add(affectedObject);
        affect.rotateAnchor = connectionAnchor;

        // AllAbilities.Add("RotatorRotate", new GameComponentAbility("RotatorRotate", AbiliityStart, AbilityRunning, AbiliityEnd, this));
    }

    public void AbiliityStart() {
        affect.InvokeStart();
    }
    public void AbilityRunning() {
        affect.Invoke();
    }
    public void AbiliityEnd() {
        affect.SetToDefault();
    }
}
