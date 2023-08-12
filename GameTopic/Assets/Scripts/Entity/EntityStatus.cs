using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IStatus {
    public Type Type { get; }
}


public class HealthStatus : HealthController<float>, IStatus {
    public Type Type { get; } = typeof(HealthStatus);

    public HealthStatus() : base(0) { }
    public HealthStatus(float max, float current, float min) : base(max, current, min) { }
}

public class SingleTarget : IStatus {
    public Type Type { get; } = typeof(SingleTarget);
    public Stack<Entity> TargetStack { get; set; } = new Stack<Entity>();
}

public class AttackStatus : IStatus {
    public Type Type { get; } = typeof(AttackStatus);
    public float Value { get; set; } = 50f;

    public void Caculate() { Value = 5566; }
}