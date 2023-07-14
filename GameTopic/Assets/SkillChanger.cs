using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SkillChanger : MonoBehaviour
{
    // Start is called before the first frame update
    private AssemblyRoom room; 
    private AbilityInputManager abilityInputManager;
    public InputManager inputmanager;
    public Device device;
    private bool KeySelected;
    private string key; 
    void Start()
    {
        room= GetComponent<AssemblyRoom>();
        //abilityInputManager = room.assemblySystemManager.abilityInputManager;
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
    public void Click(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!KeySelected)
            {

                foreach (AbilityInputEntry a in abilityInputManager.AbilityInputEntries)
                {
                    if (a.InputPath == ctx.control.name)
                    {
                        KeySelected = true;
                        key = ctx.control.name;
                    }
                }
            }
            else
            {
                if (ctx.control.name == key) { KeySelected = false; key = ""; return; }
                Debug.Log(ctx.control.name);
                foreach(AbilityInputEntry a in abilityInputManager.AbilityInputEntries)
                {
                    if (a.InputPath == ctx.control.name)
                    {
                        a.SetInputPath(key);
                    }
                }
                foreach (AbilityInputEntry a in abilityInputManager.AbilityInputEntries)
                {
                    if (a.InputPath == key)
                    {
                        a.SetInputPath(ctx.control.name);
                    }
                }
                key = "";
                KeySelected=false;
            }

        }
    }
}
