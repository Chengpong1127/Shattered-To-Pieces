using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : BaseCoreComponent, IAffectObjectOwner {
    public IAffectedObject affectedObject { get { return affectedObjectInstance; } }
    public AffectedObjectBase affectedObjectInstance { get; set; }
    LoaderAffect affect { get; set; }

    [field:SerializeField] Transform rendererTransform { get; set; }

    private void Awake() {
        affect = ScriptableObject.CreateInstance<LoaderAffect>();
        affectedObjectInstance = ScriptableObject.CreateInstance<AffectedObjectBase>();

        affectedObject.collider = GetComponentInParent<Collider2D>();
        affectedObject.rigidbody = GetComponentInParent<Rigidbody2D>();
        affectedObject.joint = GetComponentInParent<AnchoredJoint2D>();
        affectedObject.transform = gameObject.transform;

        affect.owner = affectedObject;
        affect.affectedObjectList.Add(affectedObject);
        affect.colliderTransform = gameObject.transform;
        affect.transform = rendererTransform;

        AllAbilities.Add("LoaderRotate", new Ability("LoaderRotate", AbiliityStart, this));
    }

    public void AbiliityStart() {
        affect.InvokeStart();
        StartCoroutine(affect.FrameRunner());
    }
}