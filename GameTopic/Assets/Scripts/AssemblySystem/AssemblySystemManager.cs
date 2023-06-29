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
        componentMover.OnComponentDraggedTo += handleComponentDraggedTo;

        if (ControlledDevice == null){
            ControlledDevice = new GameObject("Device").AddComponent<Device>();
            var device = ControlledDevice as Device;
            device.GameComponentFactory = GameComponentFactory;
            ControlledDevice.LoadDevice(defaultDeviceInfo());
        }
    }


    private void handleComponentDraggedTo(IGameComponent component, Vector2 targetPosition)
    {
        var (gameComponent, connectorInfo) = component.GetAvailableConnection();
        if (gameComponent == null || gameComponent == component){
            Debug.Log("No available connection");
            return;
        }
        gameComponent.Connect(component, connectorInfo);
        
    }

    private DeviceInfo defaultDeviceInfo(){
        var info = new DeviceInfo();
        info.GameComponentInfoMap.Add(0, new GameComponentInfo{
            componentGUID = 0,
            connectorInfo = ConnectionInfo.NoConnection()
        });
        return info;
    }
}
