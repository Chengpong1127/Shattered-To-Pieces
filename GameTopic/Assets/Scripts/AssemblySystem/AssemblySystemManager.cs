using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssemblySystemManager : MonoBehaviour
{
    private ComponentMover componentMover;
    public UnitManager GameComponentsUnitManager;

    public void EnableAssemblyComponents(){
        componentMover.enabled = true;
        Debug.Assert(GameComponentsUnitManager != null, "GameComponentsUnitManager is null");
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var gameComponent = unit as IGameComponent;
            if (gameComponent != null){
                gameComponent.SetAssemblyMode(true);
            }
        });
    }
    public void DisableAssemblyComponents(){
        componentMover.enabled = false;
        Debug.Assert(GameComponentsUnitManager != null, "GameComponentsUnitManager is null");
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var gameComponent = unit as IGameComponent;
            if (gameComponent != null){
                gameComponent.SetAssemblyMode(false);
            }
        });
    }

    private void Awake() {
        componentMover = gameObject.AddComponent<ComponentMover>();
        componentMover.enabled = false;
        componentMover.inputManager = new InputManager();
        componentMover.OnComponentDraggedStart += handleComponentDraggedStart;
        componentMover.OnComponentDraggedEnd += handleComponentDraggedEnd;
    }
    private void handleComponentDraggedStart(IGameComponent draggedComponent, Vector2 targetPosition)
    {
        Debug.Assert(draggedComponent != null, "draggedComponent is null");
        draggedComponent.DisconnectFromParent();
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var gameComponent = unit as IGameComponent;
            if (gameComponent != null){
                gameComponent.SetAvailableForConnection(true);
            }
        });
    }


    private void handleComponentDraggedEnd(IGameComponent draggedComponent, Vector2 targetPosition)
    {
        Debug.Assert(draggedComponent != null, "draggedComponent is null");
        
        var (availableParent, connectorInfo) = draggedComponent.GetAvailableConnection();
        if (availableParent != null){
            draggedComponent.ConnectToParent(availableParent, connectorInfo);
        }
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var gameComponent = unit as IGameComponent;
            if (gameComponent != null){
                gameComponent.SetAvailableForConnection(false);
            }
        });
    }

}
