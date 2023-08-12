using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager {
    Dictionary<BuffType, Dictionary<BuffAffectedObject, Buff>> Buffs { get; set; } = new Dictionary<BuffType, Dictionary<BuffAffectedObject, Buff>>();

    public void AddBuff(Buff buff) {
        bool addBuff = true;
        buff.data.RepelBuff.ForEach(type => {
            if (Buffs.ContainsKey(type) &&
                Buffs[buff.data.Type] != null &&
                Buffs[buff.data.Type].Count > 0) {
                addBuff = false;
            }
        });
        if (!addBuff) { return; }
        if(Buffs[buff.data.Type] == null) { Buffs[buff.data.Type] = new Dictionary<BuffAffectedObject, Buff>(); }

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
}
