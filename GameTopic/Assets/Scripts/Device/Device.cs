using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device: MonoBehaviour
{
    public Dictionary<int, IGameComponent> ComponentMap = new Dictionary<int, IGameComponent>();


    public void LoadDevice(IDeviceInfo info){
        createGameComponents(info.GameComponentIDMap);
        connect(info.ConnecterMap);
    }

    public DeviceInfo DumpDevice(){
        var info = new DeviceInfo();
        


        return info;
    }
    private void createGameComponents(Dictionary<int, int> componentIDMap){
        foreach(var pair in componentIDMap){
            var component = GameComponentFactory.Instance.CreateComponent(pair.Value, pair.Key).GetComponent<IGameComponent>();
            Debug.Assert(component != null);
            component.ComponentID = pair.Key;
            ComponentMap.Add(component.ComponentID, component);
        }
    }
    private void connect(Dictionary<int, ConnectorPoint> connecterMap){
        foreach(var pair in connecterMap){
            var component = ComponentMap[pair.Key];
            var otherComponent = ComponentMap[pair.Value.ComponentID];
            Debug.Assert(component != null);
            Debug.Assert(otherComponent != null);
            component.Connect(otherComponent, pair.Value.TargetID);
        }
    }

    public DeviceInfo DumpDeviceInfo(){
        var info = new DeviceInfo();
        info.GameComponentIDMap = new Dictionary<int, int>();
        info.ConnecterMap = new Dictionary<int, ConnectorPoint>();
        foreach(var componentID in ComponentMap.Keys){
            var component = ComponentMap[componentID];
            info.GameComponentIDMap.Add(componentID, component.ComponentGUID);
            //info.ConnecterMap.Add(componentID, component.GetConnectorPoints());
        }
        return info;
    }


}
