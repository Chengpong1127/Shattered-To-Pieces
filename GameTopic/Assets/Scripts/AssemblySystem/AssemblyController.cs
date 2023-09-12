using UnityEngine.InputSystem;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Netcode;
[RequireComponent(typeof(DraggableController))]
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
    private InputAction flipAction;
    private InputAction rotateAction;

    public void Initialize(
        Func<ulong[]> getDraggableGameObjectIDs, 
        Func<ulong[]> getConnectableGameObjectIDs, 
        InputAction dragAction, 
        InputAction flipAction, 
        InputAction rotateAction,
        float rotationUnit = 0.3f
    ){
        DraggableController = gameObject.GetComponent<DraggableController>();
        DraggableController.Initialize(getDraggableGameObjectIDs, dragAction, Camera.main);
        DraggableController.OnDragStart += HandleComponentDraggedStartServerRpc;
        DraggableController.OnDragEnd += HandleComponentDraggedEndServerRpc;
        GetConnectableGameObject = getConnectableGameObjectIDs;
        RotationUnit = rotationUnit;
        if (IsOwner){
            this.flipAction = flipAction;
            this.rotateAction = rotateAction;
            this.flipAction.started += FlipHandler;
            this.rotateAction.started += RotateHandler;
        }
        this.flipAction.Disable();
        this.rotateAction.Disable();
    }
    void OnEnable()
    {
        DraggableController.enabled = true;
        if(IsOwner){
            flipAction.Enable();
            rotateAction.Enable();
        }
    }
    void OnDisable()
    {
        DraggableController.enabled = false;
        if(IsOwner){
            flipAction.Disable();
            rotateAction.Disable();
        }
    }

    private void FlipHandler(InputAction.CallbackContext context){
        if (IsOwner){
            if (DraggableController.IsDragging.Value == true){
                var componentID = DraggableController.DraggedComponentID.Value;
                FlipServerRpc(componentID);
            }
        }

    }
    [ServerRpc]
    private void FlipServerRpc(ulong componentID){
        var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IAssemblyable>();
        Debug.Assert(component != null, "component is null");
        component.FlipTransform.localScale = new Vector3(-component.FlipTransform.localScale.x, component.FlipTransform.localScale.y, component.FlipTransform.localScale.z);
    }
    private void RotateHandler(InputAction.CallbackContext context){
        if (IsOwner){
            var value = context.ReadValue<float>();
            if (DraggableController.IsDragging.Value == true){
                var componentID = DraggableController.DraggedComponentID.Value;
                AddRotationServerRpc(componentID, value * RotationUnit);
            }
            
        }
    }
    [ServerRpc]
    private void AddRotationServerRpc(ulong componentID, float rotation){
        var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IAssemblyable>();
        Debug.Assert(component != null, "component is null");
        component.RotationTransform.Rotate(new Vector3(0, 0, rotation));
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
