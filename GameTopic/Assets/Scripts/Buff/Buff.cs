using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Buff {
    public BuffData data { get; set; }

    public Buff() {
        data = BuffFactory.Instance.RequireBuffData();
        data.Type = typeof(Buff);
    }
    ~Buff() {
        BuffFactory.Instance.ReleaseBuffData(data);
    }
    public virtual void Init() {
        data.Status = BuffExecutionStatus.Running;
        this.TriggerEvent(EventName.BuffEvents.OnTrigger(data.Target,nameof(data.Type)),this); // <-- trigger event when this buff can be executed.
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
        this.TriggerEvent(EventName.BuffEvents.RemoveBuff, this);
    }
}

#region exampleBuff

public class TakeDamage : Buff {
    AttackStatus Attacker = null;
    HealthStatus Affected = null;

    public TakeDamage() {
        data.Name = "TakeDamage";
        data.Type = typeof(TakeDamage);
    }

    public override void Init() {
        Attacker = data.Creator.GetStatus(typeof(AttackStatus)) as AttackStatus;
        Affected = data.Target.GetStatus(typeof(HealthStatus)) as HealthStatus;

        if(Attacker == null || Affected == null) { Finish(); }

        data.Status = BuffExecutionStatus.Running;
        Execute();
    }

    public override void Execute() {
        Affected.TakeDamage(Attacker.Value);

        data.Status = BuffExecutionStatus.Finish;
        Finish();
    }
}

public class Attack : Buff {
    Entity AttackTarget = null;
    AttackStatus Attacker = null;
    public Attack() {
        data.Name = "Attack";
        data.Type = typeof(Attack);
    }

    public override void Init() {
        SingleTarget t = data.Target.GetStatus(typeof(SingleTarget)) as SingleTarget;
        Attacker = data.Target.GetStatus(typeof(AttackStatus)) as AttackStatus;
        if (Attacker == null || !t.TargetStack.TryPop(out AttackTarget)) { Finish(); } // <- make sure attacker can attack and have a attack target.

        data.Status = BuffExecutionStatus.Running;
        Execute();
    }

    public override void Execute() {
        Attacker.Caculate();// <- update attacker attack value.
        this.TriggerEvent(EventName.BuffEvents.AddBuff, typeof(TakeDamage), data.Target, AttackTarget);

        data.Status = BuffExecutionStatus.Finish;
        Finish();
    }
}

#endregion