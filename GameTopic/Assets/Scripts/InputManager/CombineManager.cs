using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CombineManager : MonoBehaviour
{
    InputManager inputManager;
    private bool isDragging=false;

    // Start is called before the first frame update
    void Start()
    {
        inputManager=new InputManager();
        inputManager.menu.Enable();
        inputManager.menu.Drag.performed+=Drag;
        inputManager.menu.Click.performed += Click;
        inputManager.menu.Drag.canceled += Drag;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Debug.Log("Dragging, Mouse Position:" + mousePosition);
        }
    }
    public void Click(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log(ctx.control.name);
        }
    }
    public void Drag(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isDragging=true;
        }else if (ctx.canceled)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Debug.Log("Cancel Dragging, Mouse Position:" + mousePosition);
            isDragging = false;
        }
    }
}
