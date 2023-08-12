using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager {
    Dictionary<BuffType, Dictionary<Entity, Buff>> Buffs { get; set; } = new Dictionary<BuffType, Dictionary<Entity, Buff>>();

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

        if (Buffs[buff.data.Type].ContainsKey(buff.data.Creater) && Buffs[buff.data.Type][buff.data.Creater].data.Layerable) {
            Buffs[buff.data.Type][buff.data.Creater].Update();
        }
        else {
            Buffs[buff.data.Type].Add(buff.data.Creater, buff);
            buff.Init();
        }
    }
    public void RemoveBuff(Buff buff) {
        if (Buffs.ContainsKey(buff.data.Type) && 
            Buffs[buff.data.Type] != null) {
            Buffs[buff.data.Type].Remove(buff.data.Creater);
        }
    }
    public Buff GetBuff(BuffType type, Entity entity) {
        if(entity == null) { return null; }
        return Buffs.ContainsKey(type) && Buffs[type].ContainsKey(entity) ? Buffs[type][entity] : null;
    }
    public bool ExistBuff(BuffType type) {
        return Buffs.ContainsKey(type) && Buffs[type].Count > 0;
    }
}
