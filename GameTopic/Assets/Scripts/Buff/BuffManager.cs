using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuffManager {
    Dictionary<Type, Dictionary<Entity, Buff>> Buffs { get; set; } = new Dictionary<Type, Dictionary<Entity, Buff>>();

    public IEnumerable<Buff> buffs {
        get {
            foreach (var (type,typedBuffs) in Buffs) {
                foreach (var (creator,buff) in typedBuffs) {
                    yield return buff;
                }
            }
        }
    }

    public void AddBuff(Buff buff) {
        bool addBuff = true;

        if(buff.data.Status != BuffExecutionStatus.Waitting) {
            Debug.LogError("buff not in Waiting status.");
            return;
        }

        buff.data.RepelBuff.ForEach(type => {
            if (Buffs.ContainsKey(type) &&
                Buffs[buff.data.Type] != null &&
                Buffs[buff.data.Type].Count > 0) {
                addBuff = false;
            }
        });
        if (!addBuff) { return; }
        if(Buffs[buff.data.Type] == null) { Buffs[buff.data.Type] = new Dictionary<Entity, Buff>(); }

        if (Buffs[buff.data.Type].ContainsKey(buff.data.Creator) && Buffs[buff.data.Type][buff.data.Creator].data.Layerable) {
            Buffs[buff.data.Type][buff.data.Creator].Update();
        }
        else {
            Buffs[buff.data.Type].Add(buff.data.Creator, buff);
            buff.Init();
        }
    }
    public void RemoveBuff(Buff buff) {
        if (Buffs.ContainsKey(buff.data.Type) && 
            Buffs[buff.data.Type] != null) {
            Buffs[buff.data.Type].Remove(buff.data.Creator);
        }
    }
    public Buff GetBuff(Type type, Entity creator) {
        if(creator == null) { return null; }
        return Buffs.ContainsKey(type) && Buffs[type].ContainsKey(creator) ? Buffs[type][creator] : null;
    }
    public bool ExistBuff(Type type) {
        return Buffs.ContainsKey(type) && Buffs[type].Count > 0;
    }
}
