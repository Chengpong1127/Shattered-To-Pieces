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
    public event Action OnPlayerLoaded;
    public event Action OnPlayerDied;
    public Device SelfDevice { get; private set; }
    protected NetworkVariable<ulong> RootNetworkObjectID = new NetworkVariable<ulong>(
        readPerm: NetworkVariableReadPermission.Owner,
        writePerm: NetworkVariableWritePermission.Server
    );
    public NetworkVariable<bool> IsAlive = new NetworkVariable<bool>(
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
        if (ServerAbilityRunner != null){
            Destroy(ServerAbilityRunner);
        }
        SelfDevice = new Device(new NetworkGameComponentFactory());
        SelfDevice.Load(DeviceInfo.CreateFromJson(json));
        ServerAbilityRunner = AbilityRunner.CreateInstance(gameObject, SelfDevice.AbilityManager, OwnerClientId);
        OnPlayerLoaded?.Invoke();
        IsAlive.Value = true;
        RootNetworkObjectID.Value = SelfDevice.RootGameComponent.NetworkObjectID;
        SelfDevice.OnDeviceDied += DeviceDiedHandler;
    }
    [ServerRpc]
    private void StartAbility_ServerRPC(int abilityNumber)
    {
        Debug.Log("StartAbility_ServerRPC: " + abilityNumber);
        ServerAbilityRunner?.StartEntryAbility(abilityNumber);
    }
    [ServerRpc]
    private void CancelAbility_ServerRPC(int abilityNumber)
    {
        Debug.Log("CancelAbility_ServerRPC: " + abilityNumber);
        ServerAbilityRunner?.CancelEntryAbility(abilityNumber);
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
    protected virtual void DeviceDiedHandler(){
        SelfDevice.OnDeviceDied -= DeviceDiedHandler;
        IsAlive.Value = false;
        OnPlayerDied?.Invoke();
        Destroy(ServerAbilityRunner);
    }
    public void ServerLoadDevice(string filename){
        if (IsServer){
            LoadLocalDeviceClientRpc(filename);
        }
    }
    [ClientRpc]
    private void LoadLocalDeviceClientRpc(string filename){
        if (IsOwner){
            var info = GetLocalDeviceInfo(filename);
            LoadDeviceServerRpc(info.ToJson());
            LocalAbilityActionMap?.Dispose();
            LocalAbilityActionMap = info.AbilityManagerInfo.GetAbilityInputActionMap();
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