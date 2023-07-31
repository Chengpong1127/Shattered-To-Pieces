using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillAffectType {
    SystemCtrl,
    Attack,
    Move,
    Other
}

[CreateAssetMenu(fileName = "SkillAffectBase", menuName = "AffectedObject/SkillAffectBase")]
public class SkillAffectBase : ScriptableObject
{
    static public SkillAffectPool Pool { get; set; } = new SkillAffectPool();

    public AffectedObjectBase owner { get; set; } = null;
    public AffectedObjectBase affectedObject { get; set; } = null;
    public SkillAffectType type { get; set; } = SkillAffectType.SystemCtrl;
    public bool IsDebuff { get; set; } = false;
    public bool Interruptible { get; set; } = true;
    public bool execute { get;set; } = false;

}

public class SkillAffectPool {
    Queue<SkillAffectBase> queue {  get; set; } = new Queue<SkillAffectBase>();
    int defaultPoolSize { get; set; } = 20;
    public SkillAffectPool() {
        for(int i = 0; i < defaultPoolSize; ++i) {
            queue.Enqueue(new SkillAffectBase());
        }
    }

    public SkillAffectBase GetElement() {
        return queue.Count > 0 ? queue.Dequeue() : new SkillAffectBase();
    }
    public void StoreElement(SkillAffectBase poolObj) {
        queue.Enqueue(poolObj);
    }
}