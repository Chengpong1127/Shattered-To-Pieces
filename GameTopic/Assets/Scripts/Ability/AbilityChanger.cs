using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class AbilityKeyChanger:IAbilityRebinder
{
    // Start is called before the first frame update
    private int abilityBID;
    public InputManager inputmanager;
    public event Action<string> OnFinishRebinding;
    public AbilityManager abilityManager { get; set; }
    private bool KeySelected;
    private string key;
    public AbilityKeyChanger(AbilityManager abilityManager)
    {
        this.abilityManager = abilityManager;
        abilityBID = -1;
        inputmanager = new InputManager();
        inputmanager.AssemblyRoom.Enable();
        inputmanager.AssemblyRoom.Click.performed += ChangeKey;
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
                    if (keyName.Length <= 1 && keyName[0] >= 'A' && keyName[0] <= 'Z')
                    {
                        abilityManager.SetBinding(abilityBID, keyName);
                        OnFinishRebinding?.Invoke(keyName);
                        Debug.Log("abilityButton:" + abilityBID + "has changed input path to " + keyName);
                        CancelRebinding();
                    }
                    else
                    {
                        Debug.Log("Skill can only be binded on A~Z");
                    }
                   
                }
            }          
        }
    }

    public void StartRebinding(int abilityButtonID)
    {
        key = abilityManager.AbilityInputEntries[abilityButtonID].InputPath;
        KeySelected = true;
        abilityBID = abilityButtonID;
        Debug.Log("KeyID: " + abilityButtonID + " is selected");
    }

    public void CancelRebinding()
    {
        abilityBID = -1;
        key = "";
        KeySelected = false;
    }
}