using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAffectedObject : IAttackable {
    public BuffManager BuffManager { get;set; } = new BuffManager();
    public float GetAttackValue() {
        this.TriggerEvent(IAttackable.Event.PreAttack.ToString(), this);
        float caculated = 0;
        this.TriggerEvent(IAttackable.Event.PostAttack.ToString(), this);

        return caculated;
    }
}

public interface IAttackable {
    public enum Event {
        PreAttack,
        PostAttack,
    }
    public float GetAttackValue();
}