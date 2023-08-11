using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff {
    public BuffData data { get; set; }
    public void Init() {
        data.Status = BuffExecutionStatus.Running;
        Execute();
    }
    public void Execute() {
        data.Status = BuffExecutionStatus.Finish;
        Finish();
    }
    public void Update() {
        // data.Status = BuffExecutionStatus.Running;
        // data.Status = BuffExecutionStatus.Finish;
    }
    public void Finish() {
        data.Target.BuffManager.RemoveBuff(this);
    }
}