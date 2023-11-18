using AbilitySystem.Authoring;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using Unity.Netcode;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTagNamespace.Authoring;
using System.Collections;

public class ControlRoom : BaseCoreComponent, IDeviceRoot {
    public Device Device { get; set; }

    int currentRunningAbility = 0;
    bool useAbility = false;
    Coroutine longStayCoroutine = null;

    override protected void Awake() {
        base.Awake();

        GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility += StartAbilityListener;
        GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility += CancelAbilityListener;
    }
    override protected void Start() {
        if (longStayCoroutine != null) { StopCoroutine(longStayCoroutine); }
        longStayCoroutine = StartCoroutine(LongStayProcess());
    }
    override public void OnDestroy() {
        GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility -= StartAbilityListener;
        GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility -= CancelAbilityListener;

        base.OnDestroy();
    }

    void StartAbilityListener(int skillID) {
        if (!IsOwner) { return; }
        StartAbilityListenerServerRpc();
    }
    [ServerRpc]
    void StartAbilityListenerServerRpc() {
        if (currentRunningAbility == 0) { BodyAnimator.SetTrigger("OnUseSkill"); }
        currentRunningAbility++;
        useAbility = true;
    }

    void CancelAbilityListener(int skillID) {
        if (!IsOwner) { return; }
        CancelAbilityListenerServerRpc();
    }
    [ServerRpc]
    void CancelAbilityListenerServerRpc() {
        currentRunningAbility--;
        if (currentRunningAbility == 0) {
            BodyAnimator.SetTrigger("Idle");
            if (longStayCoroutine != null) { StopCoroutine(longStayCoroutine); }
            longStayCoroutine = StartCoroutine(LongStayProcess());
        }
    }

    IEnumerator LongStayProcess() {
        useAbility = false;
        while (true) {
            yield return new WaitForSeconds(10);
            if (useAbility) { break; }
            BodyAnimator.SetTrigger("LongStayIdle");
            yield return new WaitUntil(() => BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("LongStayIdle"));
            yield return new WaitUntil(() => !BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("LongStayIdle"));
        }

        longStayCoroutine = null;
        yield return null;
    }
}