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
    private IGameComponentFactory GameComponentFactory;

    private AbilityRunner abilityRunner;
    public InputActionMap AbilityActionMap { get; private set; }
    public AssemblyController AssemblyController;
    private PlayerInput playerInput;
    

    [ServerRpc]
    private void LoadDeviceServerRpc(string json)
    {
        SelfDevice = new Device(GameComponentFactory);
        SelfDevice.Load(DeviceInfo.CreateFromJson(json));
        abilityRunner = AbilityRunner.CreateInstance(gameObject, SelfDevice.AbilityManager, OwnerClientId);
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
            playerInput = GetComponent<PlayerInput>();
            DeviceInfo info = GetLocalDeviceInfo();
            LoadDeviceServerRpc(info.ToJson());
            AbilityActionMap = info.AbilityManagerInfo.GetAbilityInputActionMap();
            GameEvents.AbilityRunnerEvents.OnLocalStartAbility += StartAbility_ServerRPC;
            GameEvents.AbilityRunnerEvents.OnLocalCancelAbility += CancelAbility_ServerRPC;
            playerInput.SwitchCurrentActionMap("Game");
        }
        InitAssemblyControl();
    }
    private void InitAssemblyControl(){
        AssemblyController = GetComponent<AssemblyController>();
        Debug.Assert(AssemblyController != null, "AssemblyController is null");
        AssemblyController.Initialize(GetConnectableGameObjects, playerInput.currentActionMap.FindAction("DragComponent"), playerInput.currentActionMap.FindAction("FlipComponent"), 45f);
    }

    private IGameComponent[] GetConnectableGameObjects(){
        var colliders = Physics2D.OverlapCircleAll(GetLocalRootGameObject().transform.position, AssemblyRange);
        return colliders.Select(collider => collider.GetComponentInParent<IGameComponent>())
            .Where(component => component != null)
            .ToArray();
    }
    private GameObject GetLocalRootGameObject(){
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(RootNetworkObjectID.Value, out var networkObject);
        return networkObject.gameObject;
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

    void OnDrawGizmos()
    {
        if (AssemblyController != null && AssemblyController.enabled){
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetLocalRootGameObject().transform.position, AssemblyRange);
        }
    }
}
