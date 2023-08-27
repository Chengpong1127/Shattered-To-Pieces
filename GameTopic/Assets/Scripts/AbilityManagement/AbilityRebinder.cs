

using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class AbilityRebinder : IAbilityRebinder
{
    public InputActionMap AbilityActionMap { get; private set; }
    public event Action<string> OnFinishRebinding;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private readonly AbilityManager _abilityManager;
    private bool actionEnabled;
    public AbilityRebinder(AbilityManager abilityManager, InputActionMap actions)
    {
        _abilityManager = abilityManager ?? throw new ArgumentNullException(nameof(abilityManager));
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
        }
    }

    public void StartRebinding(int abilityButtonID)
    {
        if (rebindingOperation != null){
            CancelRebinding();
        }
        var action = AbilityActionMap.FindAction("Ability" + abilityButtonID.ToString(), true);
        actionEnabled = action.enabled;
        action.Disable();
        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnComplete(operation => RebindingComplete(abilityButtonID, operation))
            .Start();

    }
    private void RebindingComplete(int abilityID, InputActionRebindingExtensions.RebindingOperation operation){
        _abilityManager.SetBinding(abilityID, operation.action.bindings[0].effectivePath);
        OnFinishRebinding?.Invoke(operation.action.bindings[0].effectivePath);
        this.TriggerEvent(EventName.AbilityRebinderEvents.OnFinishRebinding, abilityID, operation.action.bindings[0].effectivePath);
        if (actionEnabled) rebindingOperation.action.Enable();
        rebindingOperation.Dispose();
        rebindingOperation = null;
    }
}