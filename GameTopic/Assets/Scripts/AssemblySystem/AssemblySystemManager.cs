using UnityEngine.InputSystem;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public class AssemblyController : MonoBehaviour
{
    private DraggableController DraggableController;
    private Func<IGameComponent[]> GetConnectableGameObject { get; set; }
    public float SingleRotationAngle { get; private set; }

    private bool _scrollAvailable = true;
    private IGameComponent[] connectableComponents;

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

    public static AssemblyController CreateInstance(GameObject where, Func<IGameComponent[]> getConnectableGameObject, InputAction dragAction, InputAction flipAction, float SingleRotationAngle = 45f){
        var instance = where.AddComponent<AssemblyController>();
        instance.DraggableController = DraggableController.CreateInstance(where, getConnectableGameObject, dragAction, Camera.main);
        instance.GetConnectableGameObject = getConnectableGameObject;
        instance.SingleRotationAngle = SingleRotationAngle > 0 && SingleRotationAngle < 360 ? SingleRotationAngle : throw new ArgumentException(nameof(SingleRotationAngle));
        if (flipAction != null) flipAction.started += instance.FlipHandler;
        else Debug.LogWarning("flipAction is null");
        return instance;
    }
    void OnEnable()
    {
        if (DraggableController != null)
        {
            DraggableController.enabled = true;
        }
    }
    void OnDisable()
    {
        DraggableController.enabled = false;
    }

    private void FlipHandler(InputAction.CallbackContext context){
        DraggableController.DraggedComponent?.ToggleXScale();
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
        connectableComponents = GetConnectableGameObject();
        connectableComponents.ToList().Remove(component);
        SetAvailableForConnection(connectableComponents, true);
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
        SetAvailableForConnection(connectableComponents, false);
        connectableComponents = null;
        OnGameComponentDraggedEnd?.Invoke(component);
    }
    private void SetAvailableForConnection(ICollection<IGameComponent> components, bool available){
        foreach (var component in components)
        {
            component.SetAvailableForConnection(available);
        }
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
