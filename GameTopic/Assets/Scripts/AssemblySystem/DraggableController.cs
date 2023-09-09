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
    public event Action<ulong, Vector2> OnScrollWhenDragging;
    private Func<ulong[]> GetDraggableObjects;

    private NetworkVariable<ulong> DraggedComponentID = new NetworkVariable<ulong>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public void Initialize(Func<ulong[]> GetDraggableObjects, InputAction dragAction, Camera mainCamera){
        DragAction = dragAction ?? throw new ArgumentNullException(nameof(dragAction));
        MainCamera = mainCamera;
        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera is null");
        }
        this.GetDraggableObjects = GetDraggableObjects ?? throw new ArgumentNullException(nameof(GetDraggableObjects));

        DragAction.started += DragStarted;
        DragAction.canceled += DragCanceled;
        DragAction.Enable();
    }
    public ulong? GetDraggableIDUnderMouse()
    {
        var gameObject = Utils.GetGameObjectUnderMouse();
        return gameObject?.GetComponentInParent<IDraggable>().NetworkObjectID;
    }

    protected void Update() {
        if (IsOwner && DraggedComponentID.Value != 0)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPoint = MainCamera.ScreenToWorldPoint(mousePosition);
            SetDraggablePositionServerRpc(DraggedComponentID.Value, worldPoint);
            OnScrollWhenDragging?.Invoke(DraggedComponentID.Value, Mouse.current.scroll.ReadValue());
        }
    }
    protected void OnEnable() {
        DragAction?.Enable();
        
    }
    protected void OnDisable() {
        DragAction?.Disable();
    }
    private void DragStarted(InputAction.CallbackContext ctx)
    {
        Debug.Log("DragStarted");
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        DraggedComponentID.Value = GetDraggableIDUnderMouse() ?? 0;
        var worldPoint = MainCamera.ScreenToWorldPoint(mousePosition);
        if (DraggedComponentID.Value != 0 && GetDraggableObjects().Contains(DraggedComponentID.Value))
        {
            OnDragStart?.Invoke(DraggedComponentID.Value, worldPoint);
        }else{
            DraggedComponentID.Value = 0;
        }
        
    }
    private void DragCanceled(InputAction.CallbackContext ctx)
    {
        Debug.Log("DragCanceled");
        if (DraggedComponentID.Value == 0)
        {
            return;
        }
        var mousePosition = Mouse.current.position.ReadValue();
        var targetPosition = MainCamera.ScreenToWorldPoint(mousePosition);
        Vector2 targetPosition2D = new(targetPosition.x, targetPosition.y);
        OnDragEnd?.Invoke(DraggedComponentID.Value, targetPosition2D);
        DraggedComponentID.Value = 0;
    }
    [ServerRpc]
    private void SetDraggablePositionServerRpc(ulong draggableID, Vector2 targetPosition)
    {
        Debug.Assert(draggableID != 0, "draggableID is 0");
        Utils.GetLocalGameObjectByNetworkID(draggableID).transform.position = targetPosition;
    }
    public override void OnDestroy() {
        base.OnDestroy();
        DragAction.started -= DragStarted;
        DragAction.canceled -= DragCanceled;
    }
}
