using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public class DeviceInfo: IInfo
{
    public TreeInfo treeInfo;

    public string DumpJSON(){
        return JsonConvert.SerializeObject(treeInfo);
    }
    public static DeviceInfo LoadJSON(string json){
        var info = new DeviceInfo();
        info.treeInfo = JsonConvert.DeserializeObject<TreeInfo>(json);
        return info;
    }
}

public class GameComponentInfo: IInfo{
    public int componentGUID;
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

public class TreeInfo: IInfo{
    public int rootID;
    public Dictionary<int, IInfo> NodeInfoMap = new Dictionary<int, IInfo>();
    public List<(int, int)> EdgeInfoList = new List<(int, int)>();
}

public interface IInfo{

}