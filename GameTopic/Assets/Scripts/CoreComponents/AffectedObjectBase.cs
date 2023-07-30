using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AffectedObjectBase", menuName = "AffectedObject/AffectedObjectBase")]
public class AffectedObjectBase : ScriptableObject
{
    public Rigidbody2D rigidbody {
        get { return IsRigidbodyAffected ? rigidbody : null; }
        set { rigidbody = value; }
    }
    public Collider2D collider {
        get { return IsColliderAffected ? collider : null; }
        set { collider = value; }
    }
    public Transform transform {
        get { return IsTransformAffected ? transform : null; }
        set { transform = value; }
    }
    public AnchoredJoint2D joint {
        get { return IsJointAffected ? joint : null; }
        set { joint = value; }
    }
    public bool IsRigidbodyAffected { get; set; } = true;
    public bool IsColliderAffected { get; set; } = true;
    public bool IsTransformAffected { get; set; } = true;
    public bool IsJointAffected { get; set; } = true;
    public AffectedObjectData affectedObjectData { get; set; }
    List<SkillAffectBase> AffectList { get; set; } = new List<SkillAffectBase>();
}
