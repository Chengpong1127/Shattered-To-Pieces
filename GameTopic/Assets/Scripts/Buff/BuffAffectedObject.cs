using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BuffAffectedObject : Singleton<BuffAffectedObject> {
    public BuffManager BuffManager { get;set; } = new BuffManager();
}

#region exampleInheritance

public class ExampleObject : BuffAffectedObject, IAttackable, IDamageable {
    public float GetAttackValue() {
        this.TriggerEvent(IAttackable.Event.PreAttack.ToString() + this.GetHashCode(), this);
        float caculated = 0;
        this.TriggerEvent(IAttackable.Event.PostAttack.ToString(), this);

        return caculated;
    }
    public void GetDamage(float damage) {
        this.TriggerEvent(IDamageable.Event.PreTakeDamage.ToString() + this.GetHashCode(),this);
        float caculated = damage;
        this.TriggerEvent(IDamageable.Event.PostTakeDamage.ToString() + this.GetHashCode(), this);
        
        this.TriggerEvent(IDamageable.Event.PreReceiveDamage.ToString() + this.GetHashCode(), this);
        // health -= caculated
        this.TriggerEvent(IDamageable.Event.PostReceiveDamage.ToString() + this.GetHashCode(), this);


        return;
    }
}

public interface IAttackable {
    public enum Event {
        PreAttack,
        PostAttack,
    }
    public float GetAttackValue();
}

public interface IDamageable {
    public enum Event {
        PreTakeDamage,
        PostTakeDamage,
        PreReceiveDamage,
        PostReceiveDamage,
    }
    public void GetDamage(float damage);
}

#endregion