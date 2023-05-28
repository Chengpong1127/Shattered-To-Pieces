using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device: MonoBehaviour
{
    public Dictionary<int, IGameComponent> ComponentMap;


    public void LoadDevice(IDeviceInfo info){
        createGameComponents(info.GameComponentIDMap);
        connect(info.ConnecterMap);
    }
    private void createGameComponents(Dictionary<int, int> componentIDMap){
        foreach(var pair in componentIDMap){
            var component = GameComponentFactory.Instance.CreateComponent(pair.Value, pair.Key).GetComponent<IGameComponent>();
            component.ComponentID = pair.Key;
            ComponentMap.Add(component.ComponentID, component);
        }
    }
    private void connect(Dictionary<int, List<ConnectorPoint>> connecterMap){
        foreach(var pair in connecterMap){
            var component = ComponentMap[pair.Key];
            foreach(var point in pair.Value){
                var otherComponent = ComponentMap[point.ComponentID];
                component.Connect(otherComponent, point.TargetID);
            }
        }
    }

    public DeviceInfo DumpDeviceInfo(){
        var info = new DeviceInfo();
        info.GameComponentIDMap = new Dictionary<int, int>();
        info.ConnecterMap = new Dictionary<int, List<ConnectorPoint>>();
        foreach(var componentID in ComponentMap.Keys){
            var component = ComponentMap[componentID];
            info.GameComponentIDMap.Add(componentID, component.ComponentGUID);
            //info.ConnecterMap.Add(componentID, component.GetConnectorPoints());
        }
        return info;
    }


}
