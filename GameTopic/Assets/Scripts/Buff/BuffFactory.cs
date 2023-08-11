using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffFactory : Singleton<BuffFactory> {
    Queue<BuffData> _datas = new Queue<BuffData>();

    public BuffData RequireBuffData() {
        return _datas.Count > 0 ? _datas.Dequeue() : new BuffData();
    }
    public void ReleaseBuffData(BuffData obj) {
        _datas.Enqueue(obj);
    }
}
