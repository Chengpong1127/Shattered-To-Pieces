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
    public InputAction DragAction { get; private set; }
    public Camera MainCamera { get; private set; }
    public event Action<ulong, Vector2> OnDragStart;
    public event Action<ulong, Vector2> OnDragEnd;
    public ulong? DraggedComponentID => DraggedComponentID.Value == 0 ? null : DraggedComponentID.Value;
    private Func<ulong[]> GetDraggableObjects;

    private NetworkVariable<ulong> draggedComponentID = new NetworkVariable<ulong>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    
    public void Initialize(Func<ulong[]> GetDraggableObjects, InputAction dragAction, Camera mainCamera){
        if (IsOwner){
            DragAction = dragAction ?? throw new ArgumentNullException(nameof(dragAction));
            MainCamera = mainCamera;
            this.GetDraggableObjects = GetDraggableObjects ?? throw new ArgumentNullException(nameof(GetDraggableObjects));
            DragAction.started += DragStarted;
            DragAction.canceled += DragCanceled;
        }
    }

    protected void Update() {
        if (IsOwner && draggedComponentID.Value != 0)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPoint = MainCamera.ScreenToWorldPoint(mousePosition);
            SetDraggablePositionServerRpc(draggedComponentID.Value, worldPoint);
        }
    }
    protected void OnEnable() {
        if (IsOwner){
            DragAction?.Enable();
        }
        
    }
    protected void OnDisable() {
        if (IsOwner){
            DragAction?.Disable();
        }
    }
    private void DragStarted(InputAction.CallbackContext ctx)
    {
        if (IsOwner){
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            draggedComponentID.Value = GetDraggableIDUnderMouse() ?? 0;
            var worldPoint = MainCamera.ScreenToWorldPoint(mousePosition);
            if (draggedComponentID.Value != 0 && GetDraggableObjects().Contains(draggedComponentID.Value))
            {
                OnDragStart?.Invoke(draggedComponentID.Value, worldPoint);
            }else{
                draggedComponentID.Value = 0;
            }
        }
    }
    private void DragCanceled(InputAction.CallbackContext ctx)
    {
        if (IsOwner){
            if (draggedComponentID.Value == 0)
            {
                return;
            }
            var mousePosition = Mouse.current.position.ReadValue();
            var targetPosition = MainCamera.ScreenToWorldPoint(mousePosition);
            Vector2 targetPosition2D = new(targetPosition.x, targetPosition.y);
            OnDragEnd?.Invoke(draggedComponentID.Value, targetPosition2D);
            draggedComponentID.Value = 0;
        }
        
    }
    private ulong? GetDraggableIDUnderMouse()
    {
        var gameObject = Utils.GetGameObjectUnderMouse();
        return gameObject?.GetComponentInParent<IDraggable>()?.NetworkObjectID;
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
            DragAction.started -= DragStarted;
            DragAction.canceled -= DragCanceled;
        }
    }
}
