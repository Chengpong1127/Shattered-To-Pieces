using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour, ICoreComponent, IAffectObjectOwner {
    public Dictionary<string, Ability> AllAbilities { get; private set; } = new Dictionary<string, Ability>();
    public IAffectedObject affectedObject { get { return affectedObjectInstance; } }
    public AffectedObjectBase affectedObjectInstance { get; set; }
    RotatorAffect affect { get; set; }

    private void Awake() {
        affect = ScriptableObject.CreateInstance<RotatorAffect>();
        affectedObjectInstance = ScriptableObject.CreateInstance<AffectedObjectBase>();

        affectedObject.collider = GetComponentInParent<Collider2D>();
        affectedObject.rigidbody = GetComponentInParent<Rigidbody2D>();
        affectedObject.joint = GetComponentInParent<AnchoredJoint2D>();
        affectedObject.transform = gameObject.transform;

        affect.owner = affectedObject;
        affect.affectedObjectList.Add(affectedObject);

        AllAbilities.Add("RotatorRotate", new Ability("RotatorRotate", AbiliityStart, AbilityRunning, AbiliityEnd, this));
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
