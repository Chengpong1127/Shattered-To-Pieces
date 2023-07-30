using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.InputSystem.Utilities;
public class SkillChanger : MonoBehaviour
{
    // Start is called before the first frame update
    private AssemblyRoom room; 
    private AbilityManager abilityInputManager;
    public InputManager inputmanager;
    public Device device;
    public event Action<int> OnTriggeredButton;
    public event Action<int,string,string> OnTriggeredChangedKey;
    private bool KeySelected;
    private string key; 
    void Start()
    {
        room= GetComponent<AssemblyRoom>();
        abilityInputManager = room.abilityInputManager;
        device = FindObjectOfType<Device>();
        inputmanager = new InputManager();
        inputmanager.menu.Enable();
        inputmanager.menu.Click.performed += Click;
        KeySelected = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Click(InputAction.CallbackContext ctx)
    {
        string keyName=string.Empty;
        foreach(var KeyControl in Keyboard.current.allKeys)
        {
            keyName = KeyControl.displayName;
            if (KeyControl.wasPressedThisFrame)
            {
                foreach (AbilityInputEntry a in abilityInputManager.AbilityInputEntries)
                {
                    if (a.InputPath == keyName)
                    {
                        OnTriggeredButton?.Invoke(int.Parse(keyName));
                        Debug.Log("key" + keyName + "run");
                        a.TriggerAllAbilities();
                    }

                }
            }
        }
    }

    
}
