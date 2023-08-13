using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEventHandler : Singleton<BuffEventHandler> {
    public BuffEventHandler() {
        this.StartListening<Type, Entity, Entity>(EventName.BuffEvents.AddBuff,OnAddBuff);
        this.StartListening<Buff>(EventName.BuffEvents.RemoveBuff, OnRemoveBuff);
    }
    public void OnAddBuff(Type type, Entity creator, Entity target) {
        if(!type.IsSubclassOf(typeof(Buff))) { Debug.LogError("This type is not a Buff.");return; }

        Buff buff = Activator.CreateInstance(type) as Buff;
        buff.data.Creator = creator;
        buff.data.Target = target;
        target.BuffManager.AddBuff(buff);
    }
    public void OnRemoveBuff(Buff buff) {
        buff.data.Target.BuffManager.RemoveBuff(buff);
    }
}
