using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Buff {
    public BuffData data { get; set; }

    public Buff() {
        data = BuffFactory.Instance.RequireBuffData();
    }
    public virtual void Init() {
        data.Status = BuffExecutionStatus.Running;
        Execute();
    }
    public virtual void Execute() {
        data.Status = BuffExecutionStatus.Finish;
        // Finish();
    }
    public virtual void Update() {
        // data.Status = BuffExecutionStatus.Running;
        // data.Status = BuffExecutionStatus.Finish;
    }
    public virtual void Finish() {
        data.Target.BuffManager.RemoveBuff(this);
        BuffFactory.Instance.ReleaseBuffData(data);
    }

    public abstract Buff GetInstance();
}

#region exampleBuff

public class TakeDamage : Buff {
    public float damage;

    public TakeDamage() {
        data.Name = "TakeDamage";
    }

    public override void Execute() {
        IDamageable affected = data.Target as IDamageable;
        if(affected != null) {
            affected.GetDamage(damage);
        }

        data.Status = BuffExecutionStatus.Finish;
        Finish();
    }

    public override Buff GetInstance() {
        return new TakeDamage();
    }
}

public class Attack : Buff {
    Entity AttackTarget { get; set; } = null;
    float AttackValue { get; set; } = 0f;
    TakeDamage damageBuff = new TakeDamage(); // if buff won't leave on target, you can reuse them directly.

    public Attack() {
        data.Name = "Attack";
    }

    public override void Init() {
        if (AttackTarget == null) { Finish(); }
        data.Status = BuffExecutionStatus.Running;
        Execute();
    }

    public override void Execute() {
        IAttackable attacker = data.Target as IAttackable;

        if(attacker != null && AttackTarget is IDamageable) {
            
            damageBuff.data.Creator = data.Target;
            damageBuff.data.Target = AttackTarget;
            damageBuff.damage = attacker.GetAttackValue();

            AttackTarget.BuffManager.AddBuff(damageBuff);
        }

        data.Status = BuffExecutionStatus.Finish;
        Finish();
    }
    public override Buff GetInstance() {
        return new Attack();
    }
}

#endregion