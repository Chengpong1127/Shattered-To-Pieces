using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device: MonoBehaviour, IDevice
{
    public Dictionary<int, IGameComponent> ComponentMap = new Dictionary<int, IGameComponent>();


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
        foreach(var pair in GameComponentInfoMap){
            var componentID = pair.Key;
            var info = pair.Value;
            var component = GameComponentFactory.Instance.CreateComponent(info.componentGUID).GetComponent<IGameComponent>();
            Debug.Assert(component != null);
            component.ComponentID = componentID;
            component.ComponentGUID = info.componentGUID;
            //AddNewComponent(component);
        }
    }
    private void connectAllComponents(Dictionary<int, GameComponentInfo> GameComponentInfoMap){
        foreach(var pair in GameComponentInfoMap){
            if(pair.Value.connectorInfo.IsConnected == false){
                continue;
            }
            var componentID = pair.Key;
            var info = pair.Value;
            var component = ComponentMap[componentID];
            var linkedComponent = ComponentMap[info.connectorInfo.linkedConnectorID];
            Debug.Assert(component != null);
            Debug.Assert(linkedComponent != null);
            component.Connect(linkedComponent, info.connectorInfo);
        }
    }
    public void ConnectNewComponent(IGameComponent newComponent, ConnectorInfo info){
        Debug.Assert(newComponent != null);
        Debug.Assert(info.connectorID == newComponent.ComponentID);
        var ConnectedComponent = ComponentMap[info.linkedConnectorID];
        newComponent.Connect(ConnectedComponent, info);
    }
    public void ConnectNewComponent(IGameComponent newComponent){
        Debug.Assert(newComponent != null);
        newComponent.ComponentID = GetNewComponentID();
    }
    public void RemoveConnectedComponent(IGameComponent component){
        Debug.Assert(component != null);
        
    }
    public void RemoveConnectedComponent(int componentID){
        Debug.Assert(ComponentMap.ContainsKey(componentID));
        
    }
    public void ChangeConnectedInfo(IGameComponent component, ConnectorInfo info){
        Debug.Assert(component != null);
        Debug.Assert(info.connectorID == component.ComponentID);
        var ConnectedComponent = ComponentMap[info.linkedConnectorID];
  
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
        //AddNewComponent(component);
    }
    private void RemoveDisconnectedComponent(int componentID){
        //RemoveComponent(ComponentMap[componentID]);
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
            //RemoveBindingComponent(component);
        }
    }

    public void AddComponent(IGameComponent newComponent)
    {
        throw new NotImplementedException();
    }

    public void AddComponent(IGameComponent newComponent, ConnectorInfo info)
    {
        throw new NotImplementedException();
    }

    public void SetConnection(ConnectorInfo info)
    {
        throw new NotImplementedException();
    }

    void IDevice.RemoveComponent(IGameComponent component)
    {
        throw new NotImplementedException();
    }

    public void RemoveComponent(int componentID)
    {
        throw new NotImplementedException();
    }
}
