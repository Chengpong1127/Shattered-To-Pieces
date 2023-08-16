using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AffectedObjectBase", menuName = "AffectedObject/AffectedObjectBase")]
public class AffectedObjectBase : ScriptableObject, IAffectedObject {
    public Rigidbody2D rigidbody { get; set; }
    public Collider2D collider { get; set; }
    public Transform transform { get; set; }
    public AnchoredJoint2D joint { get; set; }
    public BaseCoreComponent coreComponent { get; set; }
    public bool IsRigidbodyAffected { get; set; } = true;
    public bool IsColliderAffected { get; set; } = true;
    public bool IsTransformAffected { get; set; } = true;
    public bool IsJointAffected { get; set; } = true;
    [field: SerializeField] public AffectedObjectData affectedObjectData { get; set; }
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
