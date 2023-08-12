using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IStatus {
    public Type Type { get; }
}


public class Health : HealthController<float>, IStatus {
    public Type Type { get; } = typeof(Health);

    public Health() : base(0) { }
    public Health(float max, float current, float min) : base(max, current, min) { }
}