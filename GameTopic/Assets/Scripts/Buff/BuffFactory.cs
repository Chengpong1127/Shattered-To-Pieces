using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffFactory : Singleton<BuffFactory> {
    Queue<BuffData> _datas = new Queue<BuffData>();

    public BuffData RequireBuffData() {
        if(_datas.Count == 0) { ReleaseBuffData(new BuffData()); }
        return _datas.Dequeue();
    }
    public void ReleaseBuffData(BuffData obj) {
        obj.Name = string.Empty;
        obj.Type = null;
        obj.Status = BuffExecutionStatus.Waitting;
        obj.Creator = null; // BuffAffectedObject.Instance;
        obj.Target = null;
        obj.HaveCreater = false;
        obj.Layerable = false;
        obj.Layer = 0;
        obj.LayerLimit = 0;
        obj.RepelBuff.Clear();

        _datas.Enqueue(obj);
    }
}
