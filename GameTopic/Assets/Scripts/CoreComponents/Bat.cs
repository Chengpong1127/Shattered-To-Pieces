using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour, ICoreComponent, IAffectObjectOwner
{
    public Dictionary<string, Ability> AllAbilities { get; private set; } = new Dictionary<string, Ability>();
    public IGameComponent OwnerGameComponent { get; set; }
    public IAffectedObject affectedObject { get { return affectedObjectInstance; } }
    public AffectedObjectBase affectedObjectInstance { get; set; }
    public Transform connectAnchor;
    BatAffect affect { get; set; }

    private void Awake() {
        AllAbilities = new Dictionary<string, Ability>{
            {"SwingRight", new Ability("SwingRight", SwingRight, this)},
            {"SwingLeft", new Ability("SwingLeft", SwingLeft, this)}
        };
    }
    private void Start()
    {
        affect = ScriptableObject.CreateInstance<BatAffect>();
        affectedObjectInstance = ScriptableObject.CreateInstance<AffectedObjectBase>();
        affectedObject.collider = GetComponentInParent<Collider2D>();
        affectedObject.rigidbody = GetComponentInParent<Rigidbody2D>();
        affectedObject.joint = GetComponentInParent<AnchoredJoint2D>();
        affectedObject.transform = gameObject.transform;
        affect.owner = affectedObject;
        affect.affectedObjectList.Add(affectedObject);
        affect.connectAnchor = connectAnchor;
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
