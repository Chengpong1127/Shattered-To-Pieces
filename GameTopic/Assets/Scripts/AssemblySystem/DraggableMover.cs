using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class DraggableMover: MonoBehaviour
{
    public InputAction DragAction { get; private set; }
    public Camera MainCamera { get; private set; }

    private IDraggable draggedComponent = null;
    private bool isDragging = false;
    public static DraggableMover CreateInstance(GameObject where, InputAction dragAction, Camera mainCamera){
        var instance = where.AddComponent<DraggableMover>();
        instance.DragAction = dragAction ?? throw new ArgumentNullException(nameof(dragAction));
        instance.MainCamera = mainCamera;
        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera is null");
        }
        return instance;
    }
    protected void Start() {
        DragAction.started += DragStarted;
        DragAction.canceled += DragCanceled;
    }
    protected void Update() {
        if (isDragging)
        {
            Debug.Assert(draggedComponent != null, "draggedComponent is null");
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            SetDraggablePosition(mousePosition);
            this.TriggerEvent(EventName.DraggableMoverEvents.OnScrollWhenDragging, draggedComponent ,Mouse.current.scroll.ReadValue());
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
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        draggedComponent = GetDraggableUnderMouse(mousePosition);
        
        var worldPoint = MainCamera.ScreenToWorldPoint(mousePosition);
        if (draggedComponent != null)
        {
            isDragging = true;
            Vector2 worldPoint2D = new Vector2(worldPoint.x, worldPoint.y);
            this.TriggerEvent(EventName.DraggableMoverEvents.OnDragStart, draggedComponent, worldPoint2D);
        }
        
    }
    private void DragCanceled(InputAction.CallbackContext ctx)
    {
        isDragging = false;
        if (draggedComponent == null)
        {
            return;
        }
        var mousePosition = Mouse.current.position.ReadValue();
        var targetPosition = MainCamera.ScreenToWorldPoint(mousePosition);
        Vector2 targetPosition2D = new(targetPosition.x, targetPosition.y);
        this.TriggerEvent(EventName.DraggableMoverEvents.OnDragEnd, draggedComponent, targetPosition2D);
        draggedComponent = null;
    }

    private IDraggable GetDraggableUnderMouse(Vector2 mousePosition)
    {
        var gameObject = GetGameObjectUnderMouse(mousePosition);
        if (gameObject != null)
        {
            return gameObject.GetComponentInParent<IDraggable>();
        }
        else{
            return null;
        }
    }


    private GameObject GetGameObjectUnderMouse(Vector2 mousePosition)
    {
        Vector3 worldPoint = MainCamera.ScreenToWorldPoint(mousePosition);

        RaycastHit2D hit;
        hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    private void SetDraggablePosition(Vector2 mousePosition)
    {
        var newPosition = MainCamera.ScreenToWorldPoint(mousePosition);
        newPosition.z = 0;
        if (draggedComponent != null)
        {
            draggedComponent.DraggableTransform.position = newPosition;
        }
    }
    private void OnDestroy() {
        DragAction.started -= DragStarted;
        DragAction.canceled -= DragCanceled;
    }
}
