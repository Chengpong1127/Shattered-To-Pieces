using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssemblySystemManager : MonoBehaviour
{
    private DragableMover DragableMover;
    public UnitManager GameComponentsUnitManager;
    public TempAbilityInputUI tempAbilityInputUI;
    public AbilityInputManager abilityInputManager;

    public void EnableAssemblyComponents(){
        DragableMover.enabled = true;
        Debug.Assert(GameComponentsUnitManager != null, "GameComponentsUnitManager is null");
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var gameComponent = unit as IGameComponent;
            if (gameComponent != null){
                gameComponent.SetAssemblyMode(true);
            }
        });
    }
    public void DisableAssemblyComponents(){
        DragableMover.enabled = false;
        Debug.Assert(GameComponentsUnitManager != null, "GameComponentsUnitManager is null");
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var gameComponent = unit as IGameComponent;
            if (gameComponent != null){
                gameComponent.SetAssemblyMode(false);
            }
        });
    }

    private void Awake() {
        DragableMover = gameObject.AddComponent<DragableMover>();
        DragableMover.enabled = false;
        DragableMover.inputManager = new InputManager();
        DragableMover.OnDragStart += handleComponentDraggedStart;
        DragableMover.OnDragEnd += handleComponentDraggedEnd;
        abilityInputManager = new AbilityInputManager();


        Debug.Log(abilityInputManager.AbilityInputEntries.Count);
    }
    private void Start() {
        abilityInputManager.SetAbility(0,0, new Ability("test1", ()=>{}));
        tempAbilityInputUI.abilityInputManager = abilityInputManager;
        
    }
    private void handleComponentDraggedStart(IDragable draggedComponent, Vector2 targetPosition)
    {
        Debug.Assert(draggedComponent != null, "draggedComponent is null");
        if(draggedComponent is not IGameComponent){
            return;
        }
        var component = draggedComponent as IGameComponent;
        Debug.Assert(component != null, "component is null");
        component.DisconnectFromParent();
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var gameComponent = unit as IGameComponent;
            if (gameComponent != null){
                gameComponent.SetAvailableForConnection(true);
            }
        });
    }


    private void handleComponentDraggedEnd(IDragable draggedComponent, Vector2 targetPosition)
    {
        Debug.Assert(draggedComponent != null, "draggedComponent is null");
        var component = draggedComponent as IGameComponent;
        Debug.Assert(component != null, "component is null");
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
