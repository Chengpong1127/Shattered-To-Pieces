using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillAffectType {
    SystemCtrl,
    Attack,
    Move,
    Other
}

[CreateAssetMenu(fileName = "SkillAffectBase", menuName = "SkillAffect/SkillAffectBase")]
public class SkillAffectBase : ScriptableObject, ISkillAffect {

    public IAffectedObject owner { get; set; } = null;
    public List<IAffectedObject> affectedObjectList { get; set; } = new List<IAffectedObject>();
    public SkillAffectType type { get; set; } = SkillAffectType.SystemCtrl;
    [field:SerializeField] public bool IsDebuff { get; set; } = false;
    [field: SerializeField] public bool Interruptible { get; set; } = true;
    public bool interrupt { get; set; } = false;
    public bool execute { get;set; } = false;

    virtual public void Invoke() { }
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