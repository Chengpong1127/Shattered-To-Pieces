using UnityEngine.InputSystem;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class AssemblySystemManager : MonoBehaviour
{
    private DraggableController DraggableController;
    public UnitManager GameComponentsUnitManager { get; private set; }
    public float SingleRotationAngle { get; private set; }

    private bool _scrollAvailable = true;

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

    public static AssemblySystemManager CreateInstance(GameObject where, UnitManager unitManager, InputAction dragAction, InputAction flipAction, float SingleRotationAngle = 45f){
        var instance = where.AddComponent<AssemblySystemManager>();
        instance.DraggableController = DraggableController.CreateInstance(where, dragAction, Camera.main);
        instance.GameComponentsUnitManager = unitManager ?? throw new ArgumentNullException(nameof(unitManager));
        instance.SingleRotationAngle = SingleRotationAngle > 0 && SingleRotationAngle < 360 ? SingleRotationAngle : throw new ArgumentException(nameof(SingleRotationAngle));
        flipAction.started += instance.FlipHandler;
        return instance;
    }
    void OnEnable()
    {
        DraggableController.enabled = true;
    }
    public void DisableAssemblyComponents(){
        DraggableController.enabled = false;
    }

    private void FlipHandler(InputAction.CallbackContext context){
        if (DraggableController.DraggedComponent != null){
            DraggableController.DraggedComponent.ToggleXScale();
        }
    }

    protected void Start() {
        DraggableController.OnDragStart += HandleComponentDraggedStart;
        DraggableController.OnDragEnd += HandleComponentDraggedEnd;
        DraggableController.OnScrollWhenDragging += HandleScrollWhenDragging;
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
        if (scrollValue.y != 0 && _scrollAvailable){
            var rotateAngle = scrollValue.y > 0 ? SingleRotationAngle : -SingleRotationAngle;
            component.AddZRotation(rotateAngle);
            WaitScrollCooldown();
        }
        
    }
    private async void WaitScrollCooldown(){
        _scrollAvailable = false;
        await Task.Delay(300);
        _scrollAvailable = true;
    }

    protected void OnDestroy() {
        Destroy(DraggableController);
    }

}
