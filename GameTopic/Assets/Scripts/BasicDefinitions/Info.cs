using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public class DeviceInfo: IInfo
{
    public TreeInfo treeInfo;
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
    public Dictionary<int, object> NodeInfoMap = new Dictionary<int, object>();
    public List<(int, int)> EdgeInfoList = new List<(int, int)>();
}

public interface IInfo{

}