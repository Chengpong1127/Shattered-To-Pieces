using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.InputSystem.Utilities;
public class SkillTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private AbilityRunner runner;
    public InputManager inputmanager;
    void Start()
    {
        runner= GetComponent<FormalAssemblyRoom>().AbilityRunner;
        inputmanager = new InputManager();
        inputmanager.menu.Enable();
        inputmanager.menu.Click.started += Click;
        inputmanager.menu.Click.canceled += EndClick;
    }
    void Click(InputAction.CallbackContext ctx)
    {
        foreach (var KeyControl in Keyboard.current.allKeys)
        {
            string keyName = KeyControl.displayName;
            if (KeyControl.wasPressedThisFrame)
            {
                runner.StartAbility(keyName);
            }
        }
    }
    void EndClick(InputAction.CallbackContext ctx)
    {
        foreach (var KeyControl in Keyboard.current.allKeys)
        {
            string keyName = KeyControl.displayName;
            if (KeyControl.wasReleasedThisFrame)
            {
                runner.EndAbility(keyName);
            }
        }
    }

    
}
