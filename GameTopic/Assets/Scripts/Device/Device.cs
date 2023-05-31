using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device: MonoBehaviour
{
    public Dictionary<int, IGameComponent> ComponentMap = new Dictionary<int, IGameComponent>();


    public void LoadDevice(DeviceInfo info){
        createGameComponents(info.GameComponentInfoMap);
        connect(info.GameComponentInfoMap);
    }

    public DeviceInfo DumpDevice(){
        var info = new DeviceInfo();
        foreach(var componentID in ComponentMap.Keys){
            var component = ComponentMap[componentID];
            info.GameComponentInfoMap.Add(componentID,component.DumpInfo());
        }


        return info;
    }
    private void createGameComponents(Dictionary<int, GameComponentInfo> GameComponentInfoMap){
        foreach(var pair in GameComponentInfoMap){
            var componentID = pair.Key;
            var info = pair.Value;
            var component = GameComponentFactory.Instance.CreateComponent(info.componentGUID).GetComponent<IGameComponent>();
            Debug.Assert(component != null);
            component.ComponentID = componentID;
            component.ComponentGUID = info.componentGUID;
            ComponentMap.Add(componentID, component);
        }
    }
    private void connect(Dictionary<int, GameComponentInfo> GameComponentInfoMap){
        foreach(var pair in GameComponentInfoMap){
            var componentID = pair.Key;
            var info = pair.Value;
            var component = ComponentMap[componentID];
            var otherComponent = ComponentMap[info.connectorInfo.linkedConnectorID];
            Debug.Assert(component != null);
            Debug.Assert(otherComponent != null);
            component.Connect(otherComponent, info.connectorInfo);
        }
    }
}
