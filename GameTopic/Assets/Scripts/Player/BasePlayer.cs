using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Newtonsoft.Json;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class BasePlayer : NetworkBehaviour
{
    public Device SelfDevice { get; private set; }
    protected NetworkVariable<ulong> RootNetworkObjectID = new NetworkVariable<ulong>(
        readPerm: NetworkVariableReadPermission.Owner,
        writePerm: NetworkVariableWritePermission.Server
    );
    public NetworkVariable<bool> IsLoaded = new NetworkVariable<bool>(
        false,
        writePerm: NetworkVariableWritePermission.Server
    );

    /// <summary>
    /// The ability runner on the server.
    /// </summary>
    private AbilityRunner ServerAbilityRunner;
    /// <summary>
    /// The ability input action map on the owner.
    /// </summary>
    public InputActionMap LocalAbilityActionMap { get; private set; }


    
    [ServerRpc]
    private void LoadDeviceServerRpc(string json)
    {
        SelfDevice = new Device(new NetworkGameComponentFactory());
        SelfDevice.Load(DeviceInfo.CreateFromJson(json));
        ServerAbilityRunner = AbilityRunner.CreateInstance(gameObject, SelfDevice.AbilityManager, OwnerClientId);
        IsLoaded.Value = true;
        RootNetworkObjectID.Value = SelfDevice.RootGameComponent.NetworkObjectID;
    }
    [ServerRpc]
    private void StartAbility_ServerRPC(int abilityNumber)
    {
        Debug.Log("StartAbility_ServerRPC: " + abilityNumber);
        ServerAbilityRunner.StartEntryAbility(abilityNumber);
    }
    [ServerRpc]
    private void CancelAbility_ServerRPC(int abilityNumber)
    {
        Debug.Log("CancelAbility_ServerRPC: " + abilityNumber);
        ServerAbilityRunner.CancelEntryAbility(abilityNumber);
    }
    protected virtual void Start(){
        if (IsOwner){
            GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility += StartAbility_ServerRPC;
            GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility += CancelAbility_ServerRPC;
        }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (IsOwner){
            GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility -= StartAbility_ServerRPC;
            GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility -= CancelAbility_ServerRPC;
        }
    }
    public void LoadLocalDevice(string filename){
        if (IsOwner){
            var info = GetLocalDeviceInfo(filename);
            LoadDeviceServerRpc(info.ToJson());
            LocalAbilityActionMap?.Dispose();
            LocalAbilityActionMap = info.AbilityManagerInfo.GetAbilityInputActionMap();
        }else{
            throw new InvalidOperationException("Only the owner can load device");
        }
    }

    private DeviceInfo GetLocalDeviceInfo(string filename){
        return ResourceManager.Instance.LoadLocalDeviceInfo(filename) ?? ResourceManager.Instance.LoadDefaultDeviceInfo();
    }

    public void SetPlayerPoint(Transform transform)
    {
        SelfDevice.RootGameComponent.BodyTransform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    public Transform GetTracedTransform(){
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(RootNetworkObjectID.Value, out var networkObject);
        return networkObject.transform;
    }
}
