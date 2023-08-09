using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SheetmetelAffect", menuName = "SkillAffect/SheetmetelAffect")]
public class SheetmetelAffect : SkillAffectBase
{
    public SheetmetelAffect()
    {
        this.type = SkillAffectType.SystemCtrl;
    }
}
