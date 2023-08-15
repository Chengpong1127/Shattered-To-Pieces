using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Wheels : BaseCoreComponent, IAffectObjectOwner
{

    public WheelJoint2D wheelJoint;

    private const float MoveForce = 100f;
    public IAffectedObject affectedObject { get { return affectedObjectInstance; } }
    public AffectedObjectBase affectedObjectInstance { get; set; }
    WheelAffect affect { get; set; }

    private void Awake()
    {
        affect = ScriptableObject.CreateInstance<WheelAffect>();
        affectedObjectInstance = ScriptableObject.CreateInstance<AffectedObjectBase>();
        affectedObject.collider = GetComponentInParent<Collider2D>();
        affectedObject.rigidbody = GetComponentInParent<Rigidbody2D>();
        affectedObject.joint = GetComponentInParent<AnchoredJoint2D>();
        affectedObject.transform = gameObject.transform;
        affect.owner = affectedObject;
        affect.affectedObjectList.Add(affectedObject);
        affect.wheelJoint = wheelJoint;
        Debug.Assert(wheelJoint != null, "wheelJoint is null");
        // AllAbilities = new Dictionary<string, GameComponentAbility>
        // {
        //     {"TurnLeft", new GameComponentAbility("TurnLeft",AbiliityStart, LeftRunning, AbiliityEnd,this)},
        //     {"TurnRight", new GameComponentAbility("TurnRight", AbiliityStart, RightRunning, AbiliityEnd,this)},
        // };
    }
    public void AbiliityStart()
    {
        affect.InvokeStart();
    }
    public void LeftRunning()
    {
        affect.direction = true;
        affect.Invoke();
    }
    public void RightRunning()
    {
        affect.direction = false;
        affect.Invoke();
    }
    public void AbiliityEnd()
    {
        affect.End();
    }

}

