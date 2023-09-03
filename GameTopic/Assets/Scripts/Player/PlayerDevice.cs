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
    private NetworkVariable<ulong> RootNetworkObjectID = new NetworkVariable<ulong>(
        readPerm: NetworkVariableReadPermission.Owner,
        writePerm: NetworkVariableWritePermission.Server
    );
    public bool IsLoaded => isLoaded.Value;
    private NetworkVariable<bool> isLoaded = new NetworkVariable<bool>(
        false,
        writePerm: NetworkVariableWritePermission.Server
    );
    private IGameComponentFactory GameComponentFactory;

    

    private AbilityRunner abilityRunner;
    private InputActionMap abilityActionMap;

    [ServerRpc]
    private void LoadDeviceServerRpc(string json)
    {
        SelfDevice = new Device(GameComponentFactory);
        SelfDevice.Load(DeviceInfo.CreateFromJson(json));
        abilityRunner = AbilityRunner.CreateInstance(gameObject, SelfDevice.AbilityManager);
        isLoaded.Value = true;
        RootNetworkObjectID.Value = SelfDevice.RootGameComponent.BodyNetworkObject.NetworkObjectId;
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

    public void SetPlayerPoint(Transform transform)
    {
        LoadCheck();
        SelfDevice.RootGameComponent.BodyTransform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    public Transform GetTracedTransform(){
        LoadCheck();
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(RootNetworkObjectID.Value, out var networkObject);
        return networkObject.transform;
    }
    private void LoadCheck(){
        if (!IsLoaded) throw new InvalidOperationException("The device is not loaded");
    }
}
