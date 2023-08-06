

using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class AbilityRebinder : IAbilityRebinder
{
    public InputAction[] Actions { get; set; }
    public event Action<string> OnFinishRebinding;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private readonly AbilityManager _abilityManager;

    public AbilityRebinder(AbilityManager abilityManager, InputAction[] actions)
    {
        Debug.Assert(abilityManager != null, "abilityManager is null");
        Debug.Assert(actions != null, "actions is null");
        _abilityManager = abilityManager;
        Actions = actions;

    }
    public void CancelRebinding()
    {
        if (rebindingOperation != null)
        {
            rebindingOperation.Cancel();
            rebindingOperation.Dispose();
            rebindingOperation = null;
        }
    }

    public void StartRebinding(int abilityButtonID)
    {
        var action = Actions[abilityButtonID];
        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnComplete(operation => RebindingComplete(abilityButtonID,operation))
            .Start();

    }
    private void RebindingComplete(int abilityID, InputActionRebindingExtensions.RebindingOperation operation){
        _abilityManager.SetPath(abilityID, operation.action.bindings[0].effectivePath);
        OnFinishRebinding?.Invoke(operation.action.bindings[0].effectivePath);
        rebindingOperation.Dispose();
        rebindingOperation = null;
    }
}