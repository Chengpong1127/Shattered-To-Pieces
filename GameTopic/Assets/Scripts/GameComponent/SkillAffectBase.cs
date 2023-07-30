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
    public AffectedObjectBase owner { get; set; }
    public AffectedObjectBase affectedObject { get; set; }
    public SkillAffectType type { get; set; } = SkillAffectType.SystemCtrl;
    public bool IsDebuff { get; set; } = false;
    public bool InterruptIble { get; set; } = true;

}
