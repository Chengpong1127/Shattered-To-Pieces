using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
using System.Linq;

public class DraggableController: MonoBehaviour
{
    public InputAction DragAction { get; private set; }
    public Camera MainCamera { get; private set; }
    public event Action<IDraggable, Vector2> OnDragStart;
    public event Action<IDraggable, Vector2> OnDragEnd;
    public event Action<IDraggable, Vector2> OnScrollWhenDragging;
    private Func<IDraggable[]> GetDraggableObjects;

    public IDraggable DraggedComponent = null;
    private bool isDragging = false;
    public static DraggableController CreateInstance(GameObject where, Func<IDraggable[]> GetDraggableObjects, InputAction dragAction, Camera mainCamera){
        var instance = where.AddComponent<DraggableController>();
        instance.DragAction = dragAction ?? throw new ArgumentNullException(nameof(dragAction));
        instance.MainCamera = mainCamera;
        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera is null");
        }
        instance.GetDraggableObjects = GetDraggableObjects ?? throw new ArgumentNullException(nameof(GetDraggableObjects));
        return instance;
    }
    protected void Start() {
        DragAction.started += DragStarted;
        DragAction.canceled += DragCanceled;
        DragAction.Enable();
    }
    protected void Update() {
        if (isDragging)
        {
            Debug.Assert(DraggedComponent != null, "draggedComponent is null");
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            SetDraggablePosition(mousePosition);
            OnScrollWhenDragging?.Invoke(DraggedComponent, Mouse.current.scroll.ReadValue());
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
        DraggedComponent = GetDraggableUnderMouse(mousePosition);
        var worldPoint = MainCamera.ScreenToWorldPoint(mousePosition);
        if (DraggedComponent != null && GetDraggableObjects().Contains(DraggedComponent))
        {
            isDragging = true;
            Vector2 worldPoint2D = new Vector2(worldPoint.x, worldPoint.y);
            OnDragStart?.Invoke(DraggedComponent, worldPoint2D);
        }else{
            DraggedComponent = null;
        }
        
    }
    private void DragCanceled(InputAction.CallbackContext ctx)
    {
        isDragging = false;
        if (DraggedComponent == null)
        {
            return;
        }
        var mousePosition = Mouse.current.position.ReadValue();
        var targetPosition = MainCamera.ScreenToWorldPoint(mousePosition);
        Vector2 targetPosition2D = new(targetPosition.x, targetPosition.y);
        OnDragEnd?.Invoke(DraggedComponent, targetPosition2D);
        DraggedComponent = null;
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
        if (DraggedComponent != null)
        {
            DraggedComponent.DraggableTransform.position = newPosition;
        }
    }
    private void OnDestroy() {
        DragAction.started -= DragStarted;
        DragAction.canceled -= DragCanceled;
    }
}