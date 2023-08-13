using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheetmetel : BaseCoreComponent, IAffectObjectOwner
{
    public IAffectedObject affectedObject { get { return affectedObjectInstance; } }
    public AffectedObjectBase affectedObjectInstance { get; set; }
    public Transform connectAnchor;
    public SheetmetelAffect affect;
    public void Awake()
    {
        affect = ScriptableObject.CreateInstance<SheetmetelAffect>();
        affectedObjectInstance = ScriptableObject.CreateInstance<AffectedObjectBase>();
        affectedObject.collider = GetComponentInParent<Collider2D>();
        affectedObject.rigidbody = GetComponentInParent<Rigidbody2D>();
        affectedObject.joint = GetComponentInParent<AnchoredJoint2D>();
        affectedObject.transform = gameObject.transform;
        affect.owner = affectedObject;
        affect.affectedObjectList.Add(affectedObject);
    }
}
