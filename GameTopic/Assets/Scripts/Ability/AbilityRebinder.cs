

using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class AbilityRebinder : IAbilityRebinder
{
    public InputAction[] Actions { get; set; }
    public event Action<string> OnFinishRebinding;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private readonly AbilityManager _abilityManager;
    private bool actionEnabled;
    public AbilityRebinder(AbilityManager abilityManager, InputAction[] actions)
    {
        Debug.Assert(abilityManager != null, "abilityManager is null");
        Debug.Assert(actions != null, "actions is null");
        _abilityManager = abilityManager;
        Actions = actions;
        Debug.Assert(Actions.Length == abilityManager.AbilityInputEntryNumber, "The length of actions should be the same as the length of abilityInputEntries");
        for (int i = 0; i < Actions.Length; i++)
        {
            Actions[i].RemoveAllBindingOverrides();
            if(abilityManager.AbilityInputEntries[i].InputPath != ""){
                Actions[i].AddBinding(abilityManager.AbilityInputEntries[i].InputPath);
            }
        }
    }
    public void CancelRebinding()
    {
        if (rebindingOperation != null)
        {
            rebindingOperation.Cancel();
            rebindingOperation.Dispose();
            if (actionEnabled) rebindingOperation.action.Enable(); 
            rebindingOperation = null;
        }
    }

    public void StartRebinding(int abilityButtonID)
    {
        if (rebindingOperation != null){
            CancelRebinding();
        }
        var action = Actions[abilityButtonID];
        actionEnabled = action.enabled;
        action.Disable();
        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnComplete(operation => RebindingComplete(abilityButtonID,operation))
            .Start();

    }
    private void RebindingComplete(int abilityID, InputActionRebindingExtensions.RebindingOperation operation){
        _abilityManager.SetBinding(abilityID, operation.action.bindings[0].effectivePath);
        OnFinishRebinding?.Invoke(operation.action.bindings[0].effectivePath);
        rebindingOperation.Dispose();
        if (actionEnabled) rebindingOperation.action.Enable();
        rebindingOperation = null;
    }
}