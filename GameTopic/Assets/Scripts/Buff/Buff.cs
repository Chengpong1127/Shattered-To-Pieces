using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff {
    public BuffData data { get; set; }
    public void Init() {
        Execute();
    }
    public void Execute() {
        Finish();
    }
    public void Update() { }
    public void Finish() {
        data.Target.BuffManager.RemoveBuff(this);
    }
}
