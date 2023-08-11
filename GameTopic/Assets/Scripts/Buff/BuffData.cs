using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType {
    Attack,
    Cure,
    Slow,
    UnAttackable,
    UnDebuffable
}

public enum BuffExecutionStatus {
    Waitting,
    Running,
    Finish
}

public class BuffData {
    public string Name { get;set; }
    public BuffType Type { get; set; }
    public BuffExecutionStatus Status { get; set; }
    public BuffAffectedObject Creater { get; set; }
    public BuffAffectedObject Target { get; set; }
    public bool HaveCreater { get; set; }
    public bool Layerable { get; set; }
    public int Layer { get; set; }
    public int LayerLimit { get; set; }
    public List<BuffType> RepelBuff { get;set; }
}
