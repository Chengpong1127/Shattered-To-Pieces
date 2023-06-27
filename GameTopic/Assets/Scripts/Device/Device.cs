using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device: MonoBehaviour, IDevice
{
    public Dictionary<int, IGameComponent> ComponentMap {get; private set; } = new Dictionary<int, IGameComponent>();
    public IGameComponentFactory GameComponentFactory;

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
            component.ComponentGUID = info.componentGUID;
            AddComponent(component);
        }
    }
    private void connectAllComponents(Dictionary<int, GameComponentInfo> GameComponentInfoMap){
        foreach(var pair in GameComponentInfoMap){
            if(pair.Value.connectorInfo.IsConnected == false){
                continue;
            }
            var info = pair.Value;
            SetConnection(info.connectorInfo);
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
    /// <summary>
    /// Add a new component to the device, the method will assign a new ID to the component, and set the connection to NoConnection
    /// </summary>
    /// <param name="newComponent">The component created by factory.</param>
    public void AddComponent(IGameComponent newComponent)
    {
        if(newComponent.LocalComponentID == null){
            newComponent.LocalComponentID = GetNewComponentID();
        }
        Debug.Assert(ComponentMap.ContainsKey((int)newComponent.LocalComponentID) == false, "Component ID already exists");
        ComponentMap.Add((int)newComponent.LocalComponentID, newComponent);
        SetConnection(ConnectorInfo.NoConnection((int)newComponent.LocalComponentID));
    }
    /// <summary>
    /// Set the connection for a connector.
    /// </summary>
    /// <param name="info"></param>
    public void SetConnection(ConnectorInfo info)
    {
        if(info.IsConnected == false){
            Debug.Assert(ComponentMap.ContainsKey((int)info.connectorID));
            var _component = ComponentMap[(int)info.connectorID];
            Debug.Assert(_component != null);
            _component.Disconnect();
        }else{
            Debug.Assert(ComponentMap.ContainsKey((int)info.connectorID));
            Debug.Assert(ComponentMap.ContainsKey((int)info.linkedConnectorID));
            var component = ComponentMap[(int)info.connectorID];
            var linkedComponent = ComponentMap[(int)info.linkedConnectorID];
            Debug.Assert(component != null);
            Debug.Assert(linkedComponent != null);
            component.Connect(linkedComponent, info);
        }
    }
    /// <summary>
    /// Remove a component from the device. The component need to be NoConnection before removing.
    /// </summary>
    /// <param name="component"></param>
    public void RemoveComponent(IGameComponent component)
    {
        Debug.Assert(component != null);
        Debug.Assert(ComponentMap.ContainsKey((int)component.LocalComponentID));
        ComponentMap.Remove((int)component.LocalComponentID);
        component.LocalComponentID = null;
    }
    /// <summary>
    /// Remove a component from the device. The component need to be NoConnection before removing.
    /// </summary>
    /// <param name="componentID"></param>
    public void RemoveComponent(int componentID)
    {
        Debug.Assert(ComponentMap.ContainsKey(componentID));
        RemoveComponent(ComponentMap[componentID]);
    }
}
