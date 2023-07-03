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
        GameComponentFactory = gameObject.AddComponent<GameComponentFactory>();

        componentMover = gameObject.AddComponent<ComponentMover>();
        componentMover.inputManager = new InputManager();
        componentMover.OnComponentDraggedStart += handleComponentDraggedStart;
        componentMover.OnComponentDraggedEnd += handleComponentDraggedEnd;

        ControlledDevice = createSimpleDevice();
    }
    private void handleComponentDraggedStart(IGameComponent draggedComponent, Vector2 targetPosition)
    {
        draggedComponent.DisconnectFromParent();
    }


    private void handleComponentDraggedEnd(IGameComponent draggedComponent, Vector2 targetPosition)
    {
        var (availableParent, connectorInfo) = draggedComponent.GetAvailableConnection();
        if (availableParent == null){
            return;
        }
        if (availableParent == draggedComponent){
            Debug.LogWarning("Cannot connect to self");
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
}
