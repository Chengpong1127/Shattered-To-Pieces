using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device: MonoBehaviour
{
    public Dictionary<int, IGameComponent> ComponentMap;
    public List<int> ComponentIDList;
    public Dictionary<int, List<int>> ConnecterMap;

    public void LoadDevice(IDeviceInfo info){
        createGameComponents(info.GameComponentIDMap);
        connect(info.ConnecterMap);
    }
    private void createGameComponents(Dictionary<int, int> componentIDMap){
        foreach(var pair in componentIDMap){
            var component = GameComponentFactory.Instance.CreateComponent(pair.Value).GetComponent<IGameComponent>();
            component.ComponentID = pair.Key;
            ComponentMap.Add(component.ComponentID, component);
            ComponentIDList.Add(component.ComponentID);
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

    public Device()
    {
        ComponentMap = new Dictionary<int, IGameComponent>();
        ComponentIDList = new List<int>();
    }


}
