using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssemblySystemManager : MonoBehaviour
{
    private DragableMover DragableMover;
    public UnitManager GameComponentsUnitManager;

    public void EnableAssemblyComponents(){
        DragableMover.enabled = true;
        Debug.Assert(GameComponentsUnitManager != null, "GameComponentsUnitManager is null");
    }
    public void DisableAssemblyComponents(){
        DragableMover.enabled = false;
        Debug.Assert(GameComponentsUnitManager != null, "GameComponentsUnitManager is null");
    }

    private void Awake() {
        DragableMover = gameObject.AddComponent<DragableMover>();
        DragableMover.enabled = false;
        DragableMover.inputManager = new InputManager();
        DragableMover.OnDragStart += handleComponentDraggedStart;
        DragableMover.OnDragEnd += handleComponentDraggedEnd;

    }
    private void handleComponentDraggedStart(IDraggable draggedComponent, Vector2 targetPosition)
    {
        Debug.Assert(draggedComponent != null, "draggedComponent is null");
        if(draggedComponent is not IGameComponent){
            return;
        }
        var component = draggedComponent as IGameComponent;
        Debug.Assert(component != null, "component is null");
        component.DisconnectFromParent();
        component.SetDragging(true);
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var gameComponent = unit as IGameComponent;
            if (gameComponent != null && gameComponent != component){
                gameComponent.SetAvailableForConnection(true);
            }
        });
    }


    private void handleComponentDraggedEnd(IDraggable draggedComponent, Vector2 targetPosition)
    {
        Debug.Assert(draggedComponent != null, "draggedComponent is null");
        var component = draggedComponent as IGameComponent;
        Debug.Assert(component != null, "component is null");
        component.SetDragging(false);
        var (availableParent, connectorInfo) = component.GetAvailableConnection();
        if (availableParent != null){
            component.ConnectToParent(availableParent, connectorInfo);
        }
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var gameComponent = unit as IGameComponent;
            if (gameComponent != null){
                gameComponent.SetAvailableForConnection(false);
            }
        });
    }

}
