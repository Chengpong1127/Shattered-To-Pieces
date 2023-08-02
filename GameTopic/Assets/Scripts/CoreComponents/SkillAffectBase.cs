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
public class SkillAffectBase : ScriptableObject, ISkillAffect {
    // static public SkillAffectPool Pool { get; set; } = new SkillAffectPool();

    public IAffectedObject owner { get; set; } = null;
    public IAffectedObject affectedObject { get; set; } = null;
    public SkillAffectType type { get; set; } = SkillAffectType.SystemCtrl;
    public bool IsDebuff { get; set; } = false;
    public bool Interruptible { get; set; } = true;
    public bool execute { get;set; } = false;

    public void Invoke() { }
}

public class SkillAffectPool<T> where T : new() {
    Queue<T> queue {  get; set; } = new Queue<T>();
    int defaultPoolSize { get; set; } = 20;
    public SkillAffectPool() {
        for(int i = 0; i < defaultPoolSize; ++i) {
            queue.Enqueue(new T());
        }
    }

    public T GetElement() {
        return queue.Count > 0 ? queue.Dequeue() : new T();
    }
    public void StoreElement(T poolObj) {
        queue.Enqueue(poolObj);
    }
}