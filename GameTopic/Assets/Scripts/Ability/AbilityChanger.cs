using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class AbilityKeyChanger:IAbilityKeyChanger
{
    // Start is called before the first frame update
    private int abilityBID;
    public InputManager inputmanager;
    public event Action<string> OnFinishChangeAbilityKey;
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
                        abilityManager.SetPath(abilityBID, keyName);
                        OnFinishChangeAbilityKey?.Invoke(keyName);
                        Debug.Log("abilityButton:" + abilityBID + "has changed input path to " + keyName);
                        EndChangeAbilityKey();
                    }
                    else
                    {
                        Debug.Log("Skill can only be binded on A~Z");
                    }
                   
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
        abilityBID = -1;
        key = "";
        KeySelected = false;
    }
}