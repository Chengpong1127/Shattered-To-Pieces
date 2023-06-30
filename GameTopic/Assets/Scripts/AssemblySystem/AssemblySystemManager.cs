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
    }
    private void handleComponentDraggedStart(IGameComponent draggedComponent, Vector2 targetPosition)
    {
        draggedComponent.DisconnectFromParent();
    }


    private void handleComponentDraggedEnd(IGameComponent draggedComponent, Vector2 targetPosition)
    {
        var (availableParent, connectorInfo) = draggedComponent.GetAvailableConnection();
        if (availableParent == null || availableParent == draggedComponent){
            Debug.Log("No available connection");
            return;
        }
        draggedComponent.ConnectToParent(availableParent, connectorInfo);
        
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
