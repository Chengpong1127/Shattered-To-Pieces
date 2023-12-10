

using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class AbilityRebinder : IAbilityRebinder
{
    public InputActionMap AbilityActionMap { get; private set; }
    public event Action<string> OnFinishRebinding;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private bool actionEnabled;
    public AbilityRebinder(InputActionMap actions)
    {
        AbilityActionMap = actions ?? throw new ArgumentNullException(nameof(actions));
    }
    public void CancelRebinding()
    {
        if (rebindingOperation != null)
        {
            rebindingOperation.Cancel();
            rebindingOperation.Dispose();
            if (actionEnabled) rebindingOperation.action.Enable(); 
            rebindingOperation = null;
            GameEvents.RebindEvents.OnCancelRebinding?.Invoke();
        }
    }

    public void StartRebinding(int abilityButtonID)
    {
        if (rebindingOperation != null){
            CancelRebinding();
        }
        GameEvents.RebindEvents.OnStartRebinding?.Invoke(abilityButtonID);
        var action = AbilityActionMap.FindAction("Ability" + abilityButtonID.ToString(), true);
        actionEnabled = action.enabled;
        action.Disable();
        rebindingOperation = action.PerformInteractiveRebinding()
            .OnComplete(operation => RebindingComplete(abilityButtonID, operation))
            .WithControlsExcluding("Mouse")
            .Start();

    }
    private void RebindingComplete(int abilityID, InputActionRebindingExtensions.RebindingOperation operation){
        OnFinishRebinding?.Invoke(operation.action.bindings[0].effectivePath);
        GameEvents.RebindEvents.OnFinishRebinding?.Invoke(abilityID, operation.action.bindings[0].effectivePath);
        if (actionEnabled) rebindingOperation.action.Enable();
        rebindingOperation.Dispose();
        rebindingOperation = null;
    }
}