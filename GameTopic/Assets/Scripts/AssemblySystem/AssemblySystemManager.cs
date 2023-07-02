using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssemblySystemManager : MonoBehaviour
{
    public IDevice ControlledDevice;
    IGameComponentFactory GameComponentFactory;
    ComponentMover componentMover;
    public Dictionary<Guid, IGameComponent> GlobalComponentMap {get; private set; } = new Dictionary<Guid, IGameComponent>();

    private void Start() {
        GameComponentFactory = GetComponent<IGameComponentFactory>();

        componentMover = gameObject.AddComponent<ComponentMover>();
        componentMover.inputManager = new InputManager();
        componentMover.OnComponentDraggedStart += handleComponentDraggedStart;
        componentMover.OnComponentDraggedEnd += handleComponentDraggedEnd;

        //ControlledDevice = createSimpleDevice();
    }
    private void handleComponentDraggedStart(IGameComponent draggedComponent, Vector2 targetPosition)
    {
        draggedComponent.DisconnectFromParent();
    }


    private void handleComponentDraggedEnd(IGameComponent draggedComponent, Vector2 targetPosition)
    {
        var (availableParent, connectorInfo) = draggedComponent.GetAvailableConnection();
        if (availableParent == null || availableParent == draggedComponent){
            return;
        }
        draggedComponent.ConnectToParent(availableParent, connectorInfo);
        
    }

    private IDevice createSimpleDevice(){
        var device = new GameObject("Device").AddComponent<Device>();
        device.GameComponentFactory = GameComponentFactory;
        var initComponent = GameComponentFactory.CreateGameComponentObject(0);
        device.RootGameComponent = initComponent;
        return device;
    }

    private DeviceInfo defaultDeviceInfo(){
        var info = new DeviceInfo();

        return info;
    }

    public void PrintDeviceInfo(){
        var info = ControlledDevice.Dump() as DeviceInfo;

        foreach (var (key, value) in info.treeInfo.NodeInfoMap){
            Debug.Log(key);
            Debug.Log(value);
        }

        foreach (var (from, to) in info.treeInfo.EdgeInfoList){
            Debug.Log(from);
            Debug.Log(to);
        }

    }
}
