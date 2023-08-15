using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : BaseCoreComponent, IAffectObjectOwner
{
    public IAffectedObject affectedObject { get { return affectedObjectInstance; } }
    public AffectedObjectBase affectedObjectInstance { get; set; }
    public Transform connectAnchor;
    BatAffect affect { get; set; }

    private void Awake() {
        affect = ScriptableObject.CreateInstance<BatAffect>();
        affectedObjectInstance = ScriptableObject.CreateInstance<AffectedObjectBase>();
        affectedObject.collider = GetComponentInParent<Collider2D>();
        affectedObject.rigidbody = GetComponentInParent<Rigidbody2D>();
        affectedObject.joint = GetComponentInParent<AnchoredJoint2D>();
        affectedObject.transform = gameObject.transform;
        affect.owner = affectedObject;
        affect.affectedObjectList.Add(affectedObject);
        affect.connectAnchor = connectAnchor;
        // AllAbilities = new Dictionary<string, GameComponentAbility>{
        //     {"SwingRight", new GameComponentAbility("SwingRight", SwingRight, this)},
        //     {"SwingLeft", new GameComponentAbility("SwingLeft", SwingLeft, this)}
        // };
    }
   

    public void SwingRight()
    {
        affect.clockwise = false;
        affect.Invoke();
        StartCoroutine(affect.RotateCoroutine());
    }

    public void SwingLeft()
    {
        affect.clockwise = true;
        affect.Invoke();
        StartCoroutine(affect.RotateCoroutine());
    }
}
