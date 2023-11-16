using AbilitySystem.Authoring;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using Unity.Netcode;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTagNamespace.Authoring;

public class ControlRoom : BaseCoreComponent, IDeviceRoot {
    public Device Device { get; set; }

    int currentRunningAbility = 0;
    override protected void Awake() {
        base.Awake();

        GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility += StartAbilityListener;
        GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility += CancelAbilityListener;
    }
    override public void OnDestroy() {
        GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility -= StartAbilityListener;
        GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility -= CancelAbilityListener;

        base.OnDestroy();
    }

    void StartAbilityListener(int skillID) {
        if(currentRunningAbility == 0) { BodyAnimator.SetTrigger("OnUseSkill"); }
        currentRunningAbility++;
    }
    void CancelAbilityListener(int skillID) {
        currentRunningAbility--;
        if (currentRunningAbility == 0) { BodyAnimator.SetTrigger("Idle"); }
    }
}