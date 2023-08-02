using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AffectedObjectBase", menuName = "AffectedObject/AffectedObjectBase")]
public class AffectedObjectBase : ScriptableObject, IAffectedObject {
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
    public List<ISkillAffect> affectList { get; set; } = new List<ISkillAffect>();

    public float Damege( float damage) {
        return 0.0f;
    }
    public float Heal(float heal) {
        return 0.0f;
    }
    public float BreakDefense(float damage) {
        return 0.0f;
    }
    public float AddDefense(float damage) {
        return 0.0f;
    }
}
