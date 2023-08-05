using System.Collections.Generic;
using UnityEngine;

public interface ISkillAffect {
    /// <summary>
    /// A IAffectedObject which creat this skill affect.
    /// </summary>
    public IAffectedObject owner { get; set; }
    /// <summary>
    /// A IAffectedObject which will be affect.
    /// </summary>
    public List<IAffectedObject> affectedObjectList { get; set; }
    /// <summary>
    /// Type of skill affect.
    /// </summary>
    public SkillAffectType type { get; set; }
    /// <summary>
    /// Is this skill affect a debuff ro not.
    /// </summary>
    public bool IsDebuff { get; set; }
    /// <summary>
    /// Is skill affect's inovke function can be interrupt.
    /// </summary>
    public bool Interruptible { get; set; }
    /// <summary>
    /// Interrupt Invoke function.
    /// </summary>
    public bool interrupt { get; set; }
    /// <summary>
    /// Is Invoke function should invoke.
    /// </summary>
    public bool execute { get; set; }
    /// <summary>
    /// Skill affect action.
    /// </summary>
    public void Invoke();
}
