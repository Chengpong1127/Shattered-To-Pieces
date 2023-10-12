using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class BasePlayer : NetworkBehaviour
{
    public event Action OnPlayerLoaded;
    public event Action OnPlayerDied;
    public Device SelfDevice { get; private set; }
    public NetworkVariable<ulong> RootNetworkObjectID = new NetworkVariable<ulong>(
        readPerm: NetworkVariableReadPermission.Owner,
        writePerm: NetworkVariableWritePermission.Server
    );
    [HideInInspector]
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
    private void LoadDeviceServerRpc(DeviceInfo info, Vector3 position)
    {
        loadDevice(info, position);
    }
    private async void loadDevice(DeviceInfo info, Vector3 position){
        if (ServerAbilityRunner != null){
            Destroy(ServerAbilityRunner);
        }
        SelfDevice = new Device(new NetworkGameComponentFactory());
        await SelfDevice.LoadAsync(info, position);
        ServerAbilityRunner = AbilityRunner.CreateInstance(gameObject, SelfDevice.AbilityManager, OwnerClientId);
        OnPlayerLoaded?.Invoke();
        RootNetworkObjectID.Value = SelfDevice.RootGameComponent.NetworkObjectId;
        SelfDevice.OnDeviceDied += DeviceDiedHandler;
        IsAlive.Value = true;
        SelfDevice.ForEachGameComponent(component => {
            (component as GameComponent).NetworkObject.ChangeOwnership(OwnerClientId);
        });
    }
    [ServerRpc]
    private void StartAbility_ServerRPC(int abilityNumber)
    {
        ServerAbilityRunner?.StartEntryAbility(abilityNumber);
    }
    [ServerRpc]
    private void CancelAbility_ServerRPC(int abilityNumber)
    {
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
    public void ServerLoadDevice(string filename, Vector3 position){
        if (IsServer){
            LoadLocalDeviceClientRpc(filename, position);
        }
    }
    [ClientRpc]
    protected virtual void LoadLocalDeviceClientRpc(string filename, Vector3 position){
        if (IsOwner){
            var info = GetLocalDeviceInfo(filename);
            LoadDeviceServerRpc(info, position);
            LocalAbilityActionMap?.Dispose();
            LocalAbilityActionMap = info.AbilityManagerInfo.GetAbilityInputActionMap();
            LocalAbilityActionMap.Enable();
        }
    }

    private DeviceInfo GetLocalDeviceInfo(string filename){
        return ResourceManager.Instance.LoadLocalDeviceInfo(filename) ?? ResourceManager.Instance.LoadDefaultDeviceInfo();
    }


    public Transform GetTracedTransform(){
        var obj = Utils.GetLocalGameObjectByNetworkID(RootNetworkObjectID.Value);
        return obj.transform;
    }
}
