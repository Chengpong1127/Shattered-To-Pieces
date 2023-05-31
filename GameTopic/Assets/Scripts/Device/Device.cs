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
            AddNewComponent(component);
        }
    }
    private void connect(Dictionary<int, GameComponentInfo> GameComponentInfoMap){
        foreach(var pair in GameComponentInfoMap){
            if(pair.Value.connectorInfo.linkedConnectorID == -1 || pair.Value.connectorInfo.linkedTargetID == -1){
                continue;
            }
            var componentID = pair.Key;
            var info = pair.Value;
            var component = ComponentMap[componentID];
            var otherComponent = ComponentMap[info.connectorInfo.linkedConnectorID];
            Debug.Assert(component != null);
            Debug.Assert(otherComponent != null);
            component.Connect(otherComponent, info.connectorInfo);
        }
    }
    private void AddNewComponent(IGameComponent component){
        Debug.Assert(ComponentMap.ContainsKey(component.ComponentID) == false);
        component.Connector.OnConnectConnector += AddConnectedConnector;
        component.Connector.OnConnectorIsConnected += AddConnectedConnector;
        component.Connector.OnDisconnectConnector += RemoveDisconnectedComponent;
        component.Connector.OnConnectorIsDisconnected += RemoveDisconnectedComponent;
        ComponentMap.Add(component.ComponentID, component);
        Debug.Log("Add new component");
    }
    private void RemoveBindingComponent(IGameComponent component){
        component.Connector.OnConnectConnector -= AddConnectedConnector;
        component.Connector.OnConnectorIsConnected -= AddConnectedConnector;
        component.Connector.OnDisconnectConnector -= RemoveDisconnectedComponent;
        component.Connector.OnConnectorIsDisconnected -= RemoveDisconnectedComponent;
    }
    private void RemoveComponent(IGameComponent component){
        component.Connector.OnConnectConnector -= AddConnectedConnector;
        component.Connector.OnConnectorIsConnected -= AddConnectedConnector;
        component.Connector.OnDisconnectConnector -= RemoveDisconnectedComponent;
        component.Connector.OnConnectorIsDisconnected -= RemoveDisconnectedComponent;
        ComponentMap.Remove(component.ComponentID);
        Debug.Log("Remove component");
    }
    private void AddConnectedConnector(IConnector connector){
        Debug.Assert(connector != null);
        if(connector is MonoBehaviour){
            IGameComponent gameComponent = (connector as MonoBehaviour).GetComponentInParent<IGameComponent>();
            if(gameComponent != null){
                AddConnectedComponent(gameComponent);
            }
            else{
                Debug.Log("Cannot find IGameComponent");
            }
        }
        
    }
    private void AddConnectedComponent(IGameComponent component){
        int newID = GetNewComponentID();
        component.ComponentID = newID;
        AddNewComponent(component);
    }
    private void RemoveDisconnectedComponent(int componentID){
        RemoveComponent(ComponentMap[componentID]);
    }
    private int GetNewComponentID(){
        var newID = 0;
        while(ComponentMap.ContainsKey(newID)){
            newID++;
        }
        return newID;
    }
    private void OnDestroy() {
        foreach(var componentID in ComponentMap.Keys){
            var component = ComponentMap[componentID];
            RemoveBindingComponent(component);
        }
    }
}
