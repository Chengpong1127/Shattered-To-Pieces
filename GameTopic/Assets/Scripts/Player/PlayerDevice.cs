using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Newtonsoft.Json;
using UnityEngine.InputSystem;
using System;

public class PlayerDevice : NetworkBehaviour, IPlayer
{
    public Device SelfDevice { get; private set; }
    public IGameComponentFactory GameComponentFactory { get; private set; }

    public Transform TracedTransform => SelfDevice.RootGameComponent.BodyTransform;

    private AbilityRunner abilityRunner;
    private InputActionMap abilityActionMap;
    private Transform initTransform;
    public bool IsLoaded => SelfDevice != null;
    [ServerRpc]
    private void LoadDeviceServerRpc(string json)
    {
        SelfDevice = new Device(GameComponentFactory);
        SelfDevice.Load(DeviceInfo.CreateFromJson(json));
        abilityRunner = AbilityRunner.CreateInstance(gameObject, SelfDevice.AbilityManager);
        if (initTransform != null)
        {
            SetPlayerInitPoint(initTransform);
        }
    }
    [ServerRpc]
    private void StartAbility_ServerRPC(int abilityNumber)
    {
        Debug.Log("StartAbility_ServerRPC: " + abilityNumber);
        abilityRunner.StartEntryAbility(abilityNumber);
    }
    [ServerRpc]
    private void CancelAbility_ServerRPC(int abilityNumber)
    {
        Debug.Log("CancelAbility_ServerRPC: " + abilityNumber);
        abilityRunner.CancelEntryAbility(abilityNumber);
    }
    void Awake()
    {
        GameComponentFactory = new NetworkGameComponentFactory();
    }
    void Start(){
        if (IsOwner){
            DeviceInfo info = GetLocalDeviceInfo();
            LoadDeviceServerRpc(info.ToJson());
            abilityActionMap = info.AbilityManagerInfo.GetAbilityInputActionMap();
            abilityActionMap.Enable();
            GameEvents.AbilityRunnerEvents.OnLocalStartAbility += StartAbility_ServerRPC;
            GameEvents.AbilityRunnerEvents.OnLocalCancelAbility += CancelAbility_ServerRPC;
        }
    }

    private DeviceInfo GetLocalDeviceInfo(){
        return ResourceManager.Instance.LoadLocalDeviceInfo("0") ?? ResourceManager.Instance.LoadDefaultDeviceInfo();
    }

    public void SetPlayerInitPoint(Transform transform)
    {
        if (IsLoaded)
        {
            SelfDevice.RootGameComponent.BodyTransform.SetPositionAndRotation(transform.position, transform.rotation);
        }
        else
        {
            initTransform = transform;
        }
    }
}
