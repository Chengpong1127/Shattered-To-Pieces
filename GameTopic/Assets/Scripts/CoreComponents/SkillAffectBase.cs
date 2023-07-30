using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillAffectType {
    SystemCtrl,
    Attack,
    Move,
    Other
}

[CreateAssetMenu(fileName = "SkillAffectBase", menuName = "AffectedObject/SkillAffectBase")]
public class SkillAffectBase : ScriptableObject
{
    static public SkillAffectPool Pool { get; set; } = new SkillAffectPool();

    public AffectedObjectBase owner { get; set; }
    public AffectedObjectBase affectedObject { get; set; }
    public SkillAffectType type { get; set; } = SkillAffectType.SystemCtrl;
    public bool IsDebuff { get; set; } = false;
    public bool InterruptIble { get; set; } = true;

}

public class SkillAffectPool {
    Queue<SkillAffectBase> queue {  get; set; } = new Queue<SkillAffectBase>();

    SkillAffectPool() {
        
    }
}