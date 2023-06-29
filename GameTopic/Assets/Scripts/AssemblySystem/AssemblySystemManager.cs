using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssemblySystemManager : MonoBehaviour
{
    IDevice ControlledDevice;
    IGameComponentFactory GameComponentFactory;
    ComponentMover componentMover;
    public Dictionary<Guid, IGameComponent> GlobalComponentMap {get; private set; } = new Dictionary<Guid, IGameComponent>();

    private void Start() {
        componentMover = gameObject.AddComponent<ComponentMover>();
        componentMover.inputManager = new InputManager();
        componentMover.OnComponentDraggedTo += handleComponentDraggedTo;

    }


    private void handleComponentDraggedTo(IGameComponent component, Vector2 targetPosition)
    {
        var (gameComponent, connectorInfo) = component.GetAvailableConnection();
        if (gameComponent == null){
            Debug.Log("No available connection");
            return;
        }
        Debug.Log($"Connecting {component} to {gameComponent}");
        component.Connect(gameComponent, connectorInfo);
    }
}
