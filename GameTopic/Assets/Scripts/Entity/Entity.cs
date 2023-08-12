using System.Collections.Generic;
using System;

public abstract class Entity: BaseEntity{
    Dictionary<Type, IStatus> _statuses = new Dictionary<Type, IStatus>();
    public BuffManager BuffManager= new BuffManager();

    private void Update() {
        foreach (Buff buff in BuffManager.buffs) {
            buff.Update();
        }
    }

    public IStatus GetStatus(Type type) {
        return type.IsSubclassOf(typeof(IStatus)) && _statuses.ContainsKey(type) ? _statuses[type] : null;
    }
}
