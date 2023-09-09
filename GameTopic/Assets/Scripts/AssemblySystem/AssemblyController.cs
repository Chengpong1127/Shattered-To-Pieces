using UnityEngine.InputSystem;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Netcode;

public class AssemblyController : NetworkBehaviour
{
    private DraggableController DraggableController;
    private Func<ulong[]> GetConnectableGameObject { get; set; }
    public float SingleRotationAngle { get; private set; }

    private bool _scrollAvailable = true;
    private ulong[] connectableComponentIDs;

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

    public void Initialize(Func<ulong[]> getConnectableGameObject, InputAction dragAction, InputAction flipAction, float SingleRotationAngle = 45f){
        DraggableController = gameObject.GetComponent<DraggableController>();
        Debug.Assert(DraggableController != null, "DraggableController is null");
        DraggableController.Initialize(getConnectableGameObject, dragAction, Camera.main);
        GetConnectableGameObject = getConnectableGameObject;
        this.SingleRotationAngle = SingleRotationAngle > 0 && SingleRotationAngle < 360 ? SingleRotationAngle : throw new ArgumentException(nameof(SingleRotationAngle));
        if (flipAction != null) flipAction.started += FlipHandler;
        else Debug.LogWarning("flipAction is null");
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
        //DraggableController.DraggedComponentID?.ToggleXScale();
    }

    protected void Start() {
        DraggableController.OnDragStart += HandleComponentDraggedStartServerRpc;
        DraggableController.OnDragEnd += HandleComponentDraggedEndServerRpc;
        DraggableController.OnScrollWhenDragging += HandleScrollWhenDragging;
    }
    [ServerRpc]
    private void HandleComponentDraggedStartServerRpc(ulong draggableID, Vector2 targetPosition)
    {
        var component = Utils.GetLocalGameObjectByNetworkID(draggableID)?.GetComponent<IGameComponent>();
        Debug.Assert(component != null, "component is null");
        component.DisconnectFromParent();
        component.SetDragging(true);
        connectableComponentIDs = GetConnectableGameObject();
        SetAvailableForConnection(connectableComponentIDs, true);
        OnGameComponentDraggedStart?.Invoke(component);
    }

    [ServerRpc]
    private void HandleComponentDraggedEndServerRpc(ulong draggableID, Vector2 targetPosition)
    {

        var component = Utils.GetLocalGameObjectByNetworkID(draggableID)?.GetComponent<IGameComponent>();
        Debug.Assert(component != null, "component is null");
        component.SetDragging(false);
        var (availableParent, connectorInfo) = component.GetAvailableConnection();
        if (availableParent != null){
            component.ConnectToParent(availableParent, connectorInfo);
            AfterGameComponentConnected?.Invoke(component);
        }
        SetAvailableForConnection(connectableComponentIDs, false);
        connectableComponentIDs = null;
        OnGameComponentDraggedEnd?.Invoke(component);
    }
    private void SetAvailableForConnection(ICollection<ulong> componentIDs, bool available){
        foreach (var componentID in componentIDs)
        {
            var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IGameComponent>();
            Debug.Assert(component != null, "component is null");
            component.SetAvailableForConnection(available);
        }
    }

    private void HandleScrollWhenDragging(ulong draggableID, Vector2 scrollValue){
        var component = Utils.GetLocalGameObjectByNetworkID(draggableID)?.GetComponent<IGameComponent>();
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

}
