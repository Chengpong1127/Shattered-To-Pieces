using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class AbilityChanger : MonoBehaviour,IAbilityKeyChanger
{
    // Start is called before the first frame update
    private int abilityBID;
    private AssemblyRoom room;
    private AbilityManager abilityManager;
    public InputManager inputmanager;
    public Device device;
    public event Action<string> OnFinishChangeAbility;

    private bool KeySelected;
    private string key;
    void Start()
    {
        abilityBID = -1;
        room = GetComponent<AssemblyRoom>();
        abilityManager = room.abilityInputManager;
        device = FindObjectOfType<Device>();
        inputmanager = new InputManager();
        inputmanager.menu.Enable();
        inputmanager.menu.Click.performed += ChangeKey;
        //inputmanager.menu.Click.performed += Click;
        KeySelected = false;
    }

    public void ChangeKey(InputAction.CallbackContext ctx)
    {
        if(KeySelected)
        {
            string keyName = string.Empty;
            foreach (var KeyControl in Keyboard.current.allKeys)
            {
                keyName = KeyControl.displayName;
                if (KeyControl.wasPressedThisFrame)
                {
                    abilityManager.AbilityInputEntries[abilityBID].SetInputPath(keyName);
                    EndChangeAbilityKey();
                }
            }
            

        }
    }

    public void StartChangeAbilityKey(int abilityButtonID)
    {
        key = abilityManager.AbilityInputEntries[abilityButtonID].InputPath;
        KeySelected = true;

        abilityBID = abilityButtonID;
        Debug.Log("KeyID: " + abilityButtonID + " is selected");
    }

    public void EndChangeAbilityKey()
    {
        Debug.Log("abilityButton:" + abilityBID + "has changed input path");
        OnFinishChangeAbility?.Invoke("abilityButton:" + abilityBID + "has changed input path");
        abilityBID = -1;
        key = "";
        KeySelected = false;
    }
}