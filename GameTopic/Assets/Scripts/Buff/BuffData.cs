using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BuffType {
    None,
    Attack,
    TakeDamage,
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
    public Type Type { get; set; }
    public BuffExecutionStatus Status { get; set; }
    public Entity Creator { get; set; }
    public Entity Target { get; set; }
    public bool HaveCreater { get; set; }
    public bool Layerable { get; set; }
    public int Layer { get; set; }
    public int LayerLimit { get; set; }
    public List<Type> RepelBuff { get;set; } = new List<Type>();
}
