using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
using System.Linq;
using Unity.Netcode;
public class DraggableController: NetworkBehaviour
{
    public Camera MainCamera { get; private set; }
    public event Action<ulong> OnDragStart;
    public event Action<ulong> OnDragEnd;
    private Func<ulong[]> GetDraggableObjects;
    private InputAction dragAction;
    public NetworkVariable<ulong> DraggedComponentID = new NetworkVariable<ulong>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public NetworkVariable<bool> IsDragging = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    
    public void Initialize(Func<ulong[]> GetDraggableObjects, InputAction dragAction, Camera mainCamera){
        if (IsOwner){
            this.dragAction = dragAction ?? throw new ArgumentNullException(nameof(dragAction));
            MainCamera = mainCamera;
            this.GetDraggableObjects = GetDraggableObjects ?? throw new ArgumentNullException(nameof(GetDraggableObjects));
            this.dragAction.started += DragStarted;
            this.dragAction.canceled += DragCanceled;
            this.dragAction.Disable();
        }
    }

    protected void Update() {
        if (IsOwner && IsDragging.Value)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPoint = MainCamera.ScreenToWorldPoint(mousePosition);
            SetDraggablePosition(DraggedComponentID.Value, worldPoint);
        }
    }
    protected void OnEnable() {
        if (IsOwner){
            dragAction?.Enable();
        }
        
    }
    protected void OnDisable() {
        if (IsOwner){
            dragAction?.Disable();
        }
    }
    private void DragStarted(InputAction.CallbackContext ctx)
    {
        if (IsOwner){
            var componentID = GetDraggableIDUnderMouse();
            if (componentID.HasValue && GetDraggableObjects().Contains(componentID.Value))
            {
                IsDragging.Value = true;
                DraggedComponentID.Value = componentID.Value;
                ChangeOwnership_ServerRpc(componentID.Value);
                OnDragStart?.Invoke(componentID.Value);
            }else{
                IsDragging.Value = false;
                DraggedComponentID.Value = 0;
            }
        }
    }
    [ServerRpc]
    private void ChangeOwnership_ServerRpc(ulong componentID)
    {
        var component = Utils.GetLocalGameObjectByNetworkID(componentID).GetComponent<NetworkObject>();
        component.ChangeOwnership(OwnerClientId);
    }
    [ServerRpc]
    private void RemoveOwnership_ServerRpc(ulong componentID)
    {
        var component = Utils.GetLocalGameObjectByNetworkID(componentID).GetComponent<NetworkObject>();
        component.RemoveOwnership();
    }
    private void DragCanceled(InputAction.CallbackContext ctx)
    {
        if (IsOwner){
            if (IsDragging.Value == false)
            {
                return;
            }
            RemoveOwnership_ServerRpc(DraggedComponentID.Value);
            OnDragEnd?.Invoke(DraggedComponentID.Value);
            IsDragging.Value = false;
        }
    }
    private ulong? GetDraggableIDUnderMouse()
    {
        var gameObject = Utils.GetGameObjectUnderMouse();
        return gameObject?.GetComponentInParent<IAssemblyable>()?.NetworkObjectID;
    }
    private void SetDraggablePosition(ulong draggableID, Vector2 targetPosition)
    {
        Debug.Assert(draggableID != 0, "draggableID is 0");
        Utils.GetLocalGameObjectByNetworkID(draggableID).GetComponent<IAssemblyable>().DraggableTransform.position = targetPosition;
    }
    public override void OnDestroy() {
        base.OnDestroy();
        if (IsOwner){
            dragAction.started -= DragStarted;
            dragAction.canceled -= DragCanceled;
        }
    }
}
