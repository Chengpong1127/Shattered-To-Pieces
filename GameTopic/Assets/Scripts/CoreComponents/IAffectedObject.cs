using UnityEngine;
using System.Collections.Generic;

public interface IAffectedObject {
    public Rigidbody2D rigidbody { get; set; }
    public Collider2D collider { get; set; }
    public Transform transform { get; set; }
    public AnchoredJoint2D joint { get; set; }
    public BaseCoreComponent coreComponent { get; set; }
    public bool IsRigidbodyAffected { get; set; }
    public bool IsColliderAffected { get; set; }
    public bool IsTransformAffected { get; set; }
    public bool IsJointAffected { get; set; }
    public AffectedObjectData affectedObjectData { get; set; }
    public List<ISkillAffect> affectList { get; set; }
    /// <summary>
    /// Take damage to IAffectedObject.
    /// </summary>
    /// <param name="damage">The original damage number.</param>
    /// <returns>Actural damage number.</returns>
    abstract public float Damege(float damage);
    /// <summary>
    /// Heal the IAffectedObject.
    /// </summary>
    /// <param name="heal">The original healing number.</param>
    /// <returns>Actural healing number.</returns>
    abstract public float Heal(float heal);
    /// <summary>
    /// Take damage to IAffectedObject's defense.
    /// </summary>
    /// <param name="damage">The original damage number.</param>
    /// <returns>Actural damage number.</returns>
    abstract public float BreakDefense(float damage);
    /// <summary>
    /// Heal IAffectedObject's defense.
    /// </summary>
    /// <param name="damage">The original defense number.</param>
    /// <returns>Actural defense number.</returns>
    abstract public float AddDefense(float damage);
}

