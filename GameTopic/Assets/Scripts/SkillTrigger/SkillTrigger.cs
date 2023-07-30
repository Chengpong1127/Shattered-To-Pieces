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
    private AbilityManager abilityInputManager;
    public InputManager inputmanager;
    void Start()
    {
        runner= GetComponent<FormalAssemblyRoom>().AbilityRunner;
        inputmanager = new InputManager();
        inputmanager.menu.Enable();
        inputmanager.menu.Click.performed += Click;
    }
    void Click(InputAction.CallbackContext ctx)
    {
        string keyName=string.Empty;
        foreach(var KeyControl in Keyboard.current.allKeys)
        {
            keyName = KeyControl.displayName;
            if (KeyControl.wasPressedThisFrame)
            {
                runner.StartAbility(keyName);
            }
        }
    }

    
}
