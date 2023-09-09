using UnityEngine.InputSystem;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine.Animations;
using UnityEngine.InputSystem.Controls;

public class AssemblyController : NetworkBehaviour
{
    private DraggableController DraggableController;
    private Func<ulong[]> GetConnectableGameObject { get; set; }
    public float RotationUnit { get; private set; }

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

    public void Initialize(
        Func<ulong[]> getDraggableGameObjectIDs, 
        Func<ulong[]> getConnectableGameObjectIDs, 
        InputAction dragAction, 
        InputAction flipAction, 
        InputAction rotateAction,
        float rotationUnit = 10f
    ){
        DraggableController = gameObject.GetComponent<DraggableController>();
        Debug.Assert(DraggableController != null, "DraggableController is null");
        DraggableController.Initialize(getDraggableGameObjectIDs, dragAction, Camera.main);
        GetConnectableGameObject = getConnectableGameObjectIDs;
        RotationUnit = rotationUnit;
        if (flipAction != null) flipAction.started += FlipHandler;
        if (rotateAction != null) rotateAction.started += RotateHandler;
        else Debug.LogWarning("flipAction is null");
    }
    void OnEnable()
    {
        DraggableController.enabled = true;
    }
    void OnDisable()
    {
        DraggableController.enabled = false;
    }

    private void FlipHandler(InputAction.CallbackContext context){
        //DraggableController.DraggedComponentID?.ToggleXScale();
    }
    private void RotateHandler(InputAction.CallbackContext context){
        var value = context.ReadValue<float>();
        if (value != 0){
            if (DraggableController.DraggedComponentID.HasValue){
                var componentID = DraggableController.DraggedComponentID.Value;
                AddRotationServerRpc(componentID, value * RotationUnit);
            }
        }
    }
    [ServerRpc]
    private void AddRotationServerRpc(ulong componentID, float rotation){
        var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IGameComponent>();
        Debug.Assert(component != null, "component is null");
        component.AddZRotation(rotation);
    }

    protected void Start() {
        if (IsOwner){
            DraggableController.OnDragStart += HandleComponentDraggedStartServerRpc;
            DraggableController.OnDragEnd += HandleComponentDraggedEndServerRpc;
        }
    }
    [ServerRpc]
    private void HandleComponentDraggedStartServerRpc(ulong draggableID, Vector2 targetPosition)
    {
        var component = Utils.GetLocalGameObjectByNetworkID(draggableID)?.GetComponent<IGameComponent>();
        Debug.Assert(component != null, "component is null");
        component.DisconnectFromParent();
        component.SetDraggingClientRpc(true);
        connectableComponentIDs = GetConnectableGameObject();
        SetAvailableForConnection(connectableComponentIDs, true);
        OnGameComponentDraggedStart?.Invoke(component);
    }

    [ServerRpc]
    private void HandleComponentDraggedEndServerRpc(ulong draggableID, Vector2 targetPosition)
    {

        var component = Utils.GetLocalGameObjectByNetworkID(draggableID)?.GetComponent<IGameComponent>();
        Debug.Assert(component != null, "component is null");
        component.SetDraggingClientRpc(false);
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
            component.SetAvailableForConnectionClientRpc(available);
        }
    }

}
