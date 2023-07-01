using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device: MonoBehaviour, IDevice
{
    public IGameComponentFactory GameComponentFactory;
    public IGameComponent RootGameComponent { set; get; }

    public IInfo Dump()
    {
        var tree = new Tree(RootGameComponent);
        var deviceInfo = new DeviceInfo();
        deviceInfo.treeInfo = tree.Dump();
        return deviceInfo;
    }

    public void Load(IInfo info)
    {
        Debug.Assert(info is DeviceInfo);
        var deviceInfo = info as DeviceInfo;

        var tempDictionary = createAllComponents(deviceInfo.treeInfo.NodeInfoMap);
        foreach (var (key, value) in deviceInfo.treeInfo.NodeInfoMap){
            var componentInfo = value as GameComponentInfo;
            var component = tempDictionary[key];
            component.Load(componentInfo);
        }

        var rootComponent = tempDictionary[deviceInfo.treeInfo.rootID];

        connectAllComponents(tempDictionary, deviceInfo.treeInfo.NodeInfoMap, deviceInfo.treeInfo.EdgeInfoList);
        
    }

    private Dictionary<int, IGameComponent> createAllComponents(Dictionary<int, IInfo> nodes){
        var tempDictionary = new Dictionary<int, IGameComponent>();
        foreach (var (key, value) in nodes){
            var componentInfo = value as GameComponentInfo;
            var component = GameComponentFactory.CreateGameComponentObject(componentInfo.componentGUID);
            tempDictionary.Add(key, component);
        }
        return tempDictionary;
    }

    private void connectAllComponents(Dictionary<int, IGameComponent> nodes, Dictionary<int, IInfo> infos, List<(int, int)> edges){
        foreach (var (from, to) in edges){
            var fromComponent = nodes[from];
            var toComponent = nodes[to];
            var toInfo = infos[to] as GameComponentInfo;
            
            toComponent.ConnectToParent(fromComponent, toInfo.connectionInfo);
        }
    }

}
