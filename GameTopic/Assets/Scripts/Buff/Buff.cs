using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff {
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
}

public class TakeDamage : Buff {
    public float damage;

    public TakeDamage() {
        data.Name = "TakeDamage";
        data.Type = BuffType.TakeDamage;
    }

    public override void Execute() {
        IDamageable affected = data.Target as IDamageable;
        if(affected != null) {
            affected.GetDamage(damage);
        }

        data.Status = BuffExecutionStatus.Finish;
        Finish();
    }
}

public class Attack : Buff {
    BuffAffectedObject AttackTarget { get; set; } = null;
    float AttackValue { get; set; } = 0f;

    public Attack() {
        data.Type = BuffType.Attack;
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
            TakeDamage damageBuff = new TakeDamage();
            damageBuff.data.Creater = data.Target;
            damageBuff.data.Target = AttackTarget;
            damageBuff.damage = attacker.GetAttackValue();

            AttackTarget.BuffManager.AddBuff(damageBuff);
        }

        data.Status = BuffExecutionStatus.Finish;
        Finish();
    }
}