using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Newtonsoft.Json;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class PlayerDevice : NetworkBehaviour, IPlayer
{
    public Device SelfDevice { get; private set; }
    [SerializeField]
    private float AssemblyRange = 5;
    private NetworkVariable<ulong> RootNetworkObjectID = new NetworkVariable<ulong>(
        readPerm: NetworkVariableReadPermission.Owner,
        writePerm: NetworkVariableWritePermission.Server
    );
    public bool IsLoaded => isLoaded.Value;
    private NetworkVariable<bool> isLoaded = new NetworkVariable<bool>(
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
    public AssemblyController AssemblyController;
    private PlayerInput playerInput;
    private GameObject assemblyCurtain;
    private GameObject assemblyCurtainInstance;
    [ServerRpc]
    private void LoadDeviceServerRpc(string json)
    {
        SelfDevice = new Device(new NetworkGameComponentFactory());
        SelfDevice.Load(DeviceInfo.CreateFromJson(json));
        ServerAbilityRunner = AbilityRunner.CreateInstance(gameObject, SelfDevice.AbilityManager, OwnerClientId);
        isLoaded.Value = true;
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
    void Start(){
        playerInput = GetComponent<PlayerInput>();
        if (IsOwner){
            GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility += StartAbility_ServerRPC;
            GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility += CancelAbility_ServerRPC;
        }
        InitAssemblyControl();
        assemblyCurtain = ResourceManager.Instance.LoadPrefab("AssemblyCurtain");
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
    private void InitAssemblyControl(){
        AssemblyController = GetComponent<AssemblyController>();
        Debug.Assert(AssemblyController != null, "AssemblyController is null");
        if (IsOwner || IsServer){
            AssemblyController.Initialize(
                GetDraggableNetworkIDs, 
                GetConnectableNetworkIDs, 
                playerInput.currentActionMap.FindAction("DragComponent"), 
                playerInput.currentActionMap.FindAction("FlipComponent"), 
                playerInput.currentActionMap.FindAction("RotateComponent"));
        }
    }
    [ClientRpc]
    public void ToggleAssemblyClientRpc(){
        if (IsOwner){
            AssemblyController.enabled = !AssemblyController.enabled;
            if (AssemblyController.enabled){
                assemblyCurtainInstance = Instantiate(assemblyCurtain);
            }
            else{
                if (assemblyCurtainInstance != null){
                    Destroy(assemblyCurtainInstance);
                }
            }
        }

    }

    private ulong[] GetConnectableNetworkIDs()
    {
        var colliders = Physics2D.OverlapCircleAll(Utils.GetLocalGameObjectByNetworkID(RootNetworkObjectID.Value).transform.position, AssemblyRange);
        return colliders.Select(collider => collider.GetComponentInParent<IGameComponent>())
            .Where(component => component != null)
            .Select(component => component.NetworkObjectID)
            .ToArray();
    }
    public ulong[] GetDraggableNetworkIDs()
    {
        return GetConnectableNetworkIDs().Where(id => id != RootNetworkObjectID.Value).ToArray();
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
    [ServerRpc]
    public void CleanDevice_ServerRpc(){
        SelfDevice.ForEachGameComponent(component =>
                component.CoreComponent.Die()
        );
    }
}
