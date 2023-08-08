using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class AssemblySystemManager : MonoBehaviour
{
    private DraggableMover DraggableMover;
    public UnitManager GameComponentsUnitManager { get; private set; }
    public float SingleRotationAngle { get; private set; }

        private float _scrollCounter = 0f;

    /// <summary>
    /// This event will be invoked when a game component is started to drag.
    /// </summary>
    public event Action<IGameComponent> OnGameComponentDraggedStart;
    /// <summary>
    /// This event will be invoked after a game component is dragged and released.
    /// </summary>
    public event Action<IGameComponent> OnGameComponentDraggedEnd;
    /// <summary>
    /// This event will be invoked after a game component is connected to another game component.
    /// </summary>
    public event Action<IGameComponent> AfterGameComponentConnected;

    public static AssemblySystemManager CreateInstance(GameObject where, UnitManager unitManager, InputAction dragAction, float SingleRotationAngle = 45f){
        var instance = where.AddComponent<AssemblySystemManager>();
        instance.DraggableMover = DraggableMover.CreateInstance(where, dragAction, Camera.main);
        instance.GameComponentsUnitManager = unitManager ?? throw new ArgumentNullException(nameof(unitManager));
        instance.SingleRotationAngle = SingleRotationAngle > 0 && SingleRotationAngle < 360 ? SingleRotationAngle : throw new ArgumentException(nameof(SingleRotationAngle));

        return instance;
    }

    public void EnableAssemblyComponents(){
        DraggableMover.enabled = true;
    }
    public void DisableAssemblyComponents(){
        DraggableMover.enabled = false;
    }

    private void Awake() {
        DraggableMover = gameObject.AddComponent<DraggableMover>();
        DraggableMover.enabled = false;
        DraggableMover.OnDragStart += HandleComponentDraggedStart;
        DraggableMover.OnDragEnd += HandleComponentDraggedEnd;
        DraggableMover.OnScrollWhenDragging += HandleScrollWhenDragging;

    }
    private void Update() {
        if(_scrollCounter > 0f){
            _scrollCounter -= Time.deltaTime;
            
            if (_scrollCounter <= 0f){
                _scrollCounter = 0f;
            }
        }
    }
    private void HandleComponentDraggedStart(IDraggable draggedComponent, Vector2 targetPosition)
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
            if (unit is IGameComponent gameComponent && gameComponent != component)
            {
                gameComponent.SetAvailableForConnection(true);
            }
        });
        OnGameComponentDraggedStart?.Invoke(component);
    }


    private void HandleComponentDraggedEnd(IDraggable draggedComponent, Vector2 targetPosition)
    {
        Debug.Assert(draggedComponent != null, "draggedComponent is null");
        var component = draggedComponent as IGameComponent;
        Debug.Assert(component != null, "component is null");
        component.SetDragging(false);
        var (availableParent, connectorInfo) = component.GetAvailableConnection();
        if (availableParent != null){
            component.ConnectToParent(availableParent, connectorInfo);
            AfterGameComponentConnected?.Invoke(component);
        }
        GameComponentsUnitManager.ForEachUnit((unit) => {
            if (unit is IGameComponent gameComponent)
            {
                gameComponent.SetAvailableForConnection(false);
            }
        });
        OnGameComponentDraggedEnd?.Invoke(component);
    }

    private void HandleScrollWhenDragging(IDraggable draggedComponent, Vector2 scrollValue){
        Debug.Assert(draggedComponent != null, "draggedComponent is null");
        var component = draggedComponent as IGameComponent;
        Debug.Assert(component != null, "component is null");
        if (scrollValue.y != 0 && _scrollCounter == 0f){
            var rotateAngle = scrollValue.y > 0 ? SingleRotationAngle : -SingleRotationAngle;
            component.AddZRotation(rotateAngle);
            _scrollCounter = 0.4f;
        }
        
    }

}
