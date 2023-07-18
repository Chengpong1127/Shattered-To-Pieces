using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

public class DeviceInfo: IInfo
{
    public TreeInfo<GameComponentInfo> treeInfo;
}
public class GameComponentInfo: IInfo{
    public string ComponentName;
    public ConnectionInfo connectionInfo;
    

}

// dump or load info for IConnector
public class ConnectionInfo: IInfo
{
    public int linkedTargetID;
    public float connectorRotation;
    public bool IsConnected => linkedTargetID != -1;
    public static ConnectionInfo NoConnection(){
        return new ConnectionInfo{
            linkedTargetID = -1,
            connectorRotation = 0f
        };
    }
    public override bool Equals(object obj)
    {
        return obj is ConnectionInfo info &&
               linkedTargetID == info.linkedTargetID &&
               connectorRotation == info.connectorRotation;
    }
    public override int GetHashCode()
    {
        int hashCode = 1861411795;
        hashCode = hashCode * -1521134295 + linkedTargetID.GetHashCode();
        hashCode = hashCode * -1521134295 + connectorRotation.GetHashCode();
        return hashCode;
    }

}

public interface IDumpable<T> where T: IInfo{
    public T Dump();
}
public interface ILoadable<T> where T: IInfo{
    public void Load(T info);
}
public interface IStorable: IDumpable<IInfo>, ILoadable<IInfo>{

}

public class TreeInfo<T>: IInfo where T: IInfo{
    public int rootID;
    public Dictionary<int, T> NodeInfoMap = new Dictionary<int, T>();
    public List<(int, int)> EdgeInfoList = new List<(int, int)>();

    public override bool Equals(object obj)
    {
        return obj is TreeInfo<T> info &&
                rootID == info.rootID &&
                NodeInfoMap.Count == info.NodeInfoMap.Count && !NodeInfoMap.Except(info.NodeInfoMap).Any()
                && EdgeInfoList.Count == info.EdgeInfoList.Count && !EdgeInfoList.Except(info.EdgeInfoList).Any();
    }

    public override int GetHashCode()
    {
        int hashCode = 1861411795;
        hashCode = hashCode * -1521134295 + rootID.GetHashCode();
        hashCode = hashCode * -1521134295 + NodeInfoMap.GetHashCode();
        hashCode = hashCode * -1521134295 + EdgeInfoList.GetHashCode();
        return hashCode;
    }
}

public interface IInfo{

}