using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device: MonoBehaviour, IDevice
{
    public Dictionary<int, IGameComponent> ComponentMap {get; private set; } = new Dictionary<int, IGameComponent>();
    public IGameComponentFactory GameComponentFactory;
    public IGameComponent RootGameComponent { set; get; }
    public void LoadDevice(DeviceInfo info){
        createAllComponents(info.GameComponentInfoMap);
        connectAllComponents(info.GameComponentInfoMap);
    }

    public DeviceInfo DumpDevice(){
        var info = new DeviceInfo();
        foreach(var componentID in ComponentMap.Keys){
            var component = ComponentMap[componentID];
            info.GameComponentInfoMap.Add(componentID,component.DumpInfo());
        }
        return info;
    }
    private void createAllComponents(Dictionary<int, GameComponentInfo> GameComponentInfoMap){
        Debug.Assert(GameComponentFactory != null, "GameComponentFactory is null");
        foreach(var pair in GameComponentInfoMap){
            var componentID = pair.Key;
            var info = pair.Value;
            var component = GameComponentFactory.CreateGameComponentObject(info.componentGUID);
            Debug.Assert(component != null);
            component.LocalComponentID = componentID;
        }
    }
    private void connectAllComponents(Dictionary<int, GameComponentInfo> GameComponentInfoMap){
        foreach(var pair in GameComponentInfoMap){
            if(pair.Value.connectorInfo.IsConnected == false){
                continue;
            }
            var info = pair.Value;
            //SetConnection(info.connectorInfo);
        }
    }
    /// <summary>
    /// Get a new local ID for a component.
    /// </summary>
    /// <returns></returns>
    private int GetNewComponentID(){
        var newID = 0;
        while(ComponentMap.ContainsKey(newID)){
            newID++;
        }
        return newID;
    }
}
