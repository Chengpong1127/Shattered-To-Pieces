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
    public event Action<ulong, Vector2> OnDragStart;
    public event Action<ulong, Vector2> OnDragEnd;
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
            SetDraggablePositionServerRpc(DraggedComponentID.Value, worldPoint);
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
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                var worldPoint = MainCamera.ScreenToWorldPoint(mousePosition);
                DraggedComponentID.Value = componentID.Value;
                OnDragStart?.Invoke(componentID.Value, worldPoint);
            }else{
                IsDragging.Value = false;
                DraggedComponentID.Value = 0;
            }
        }
    }
    private void DragCanceled(InputAction.CallbackContext ctx)
    {
        if (IsOwner){
            if (IsDragging.Value == false)
            {
                return;
            }
            var mousePosition = Mouse.current.position.ReadValue();
            var targetPosition = MainCamera.ScreenToWorldPoint(mousePosition);
            OnDragEnd?.Invoke(DraggedComponentID.Value, targetPosition);
            IsDragging.Value = false;
        }
    }
    private ulong? GetDraggableIDUnderMouse()
    {
        var gameObject = Utils.GetGameObjectUnderMouse();
        return gameObject?.GetComponentInParent<IAssemblyable>()?.NetworkObjectID;
    }
    [ServerRpc]
    private void SetDraggablePositionServerRpc(ulong draggableID, Vector2 targetPosition)
    {
        Debug.Assert(draggableID != 0, "draggableID is 0");
        Utils.GetLocalGameObjectByNetworkID(draggableID).transform.position = targetPosition;
    }
    public override void OnDestroy() {
        base.OnDestroy();
        if (IsOwner){
            dragAction.started -= DragStarted;
            dragAction.canceled -= DragCanceled;
        }
    }
}
