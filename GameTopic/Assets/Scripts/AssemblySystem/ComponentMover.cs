using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ComponentMover: MonoBehaviour
{
    public InputManager inputManager;
    public Camera mainCamera;
    public event Action<IGameComponent, Vector2> OnComponentDraggedStart;
    public event Action<IGameComponent, Vector2> OnComponentDraggedEnd;


    private IGameComponent draggedComponent = null;
    private bool isDragging = false;
    private void Start() {
        inputManager.menu.Enable();
        inputManager.menu.Drag.started += DragStarted;
        inputManager.menu.Drag.canceled += DragCanceled;
        mainCamera = Camera.main;
    }
    private void Update() {
        if (isDragging)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            setDraggedComponentPosition(mousePosition);
        }
    }
    private void DragStarted(InputAction.CallbackContext ctx)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        draggedComponent = getGameComponentUnderMouse(mousePosition);
        isDragging = true;
        var worldPoint = mainCamera.ScreenToWorldPoint(mousePosition);
        if (draggedComponent != null)
        {
            OnComponentDraggedStart?.Invoke(draggedComponent, worldPoint);
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
        var targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        OnComponentDraggedEnd?.Invoke(draggedComponent, targetPosition);
        draggedComponent = null;
    }

    private IGameComponent getGameComponentUnderMouse(Vector2 mousePosition)
    {
        var gameObject = getGameObjectUnderMouse(mousePosition);
        if (gameObject != null)
        {
            return gameObject.GetComponentInParent<IGameComponent>();
        }
        else{
            return null;
        }
    }


    private GameObject getGameObjectUnderMouse(Vector2 mousePosition)
    {
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(mousePosition);

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

    private void setDraggedComponentPosition(Vector2 mousePosition)
    {
        var newPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        newPosition.z = 0;
        if (draggedComponent != null)
        {
            draggedComponent.CoreTransform.position = newPosition;
        }
    }
}
