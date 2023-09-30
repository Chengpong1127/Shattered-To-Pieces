using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Linq;

public class AssemblyablePlayer: BasePlayer{
    [SerializeField]
    public float AssemblyRange = 5;
    private PlayerInput playerInput;
    public AssemblyController AssemblyController;
    public AbilityRebinder AbilityRebinder;
    protected override void Start(){
        base.Start();
        playerInput = GetComponent<PlayerInput>();
        InitAssemblyControl();
        SetAssemblyMode_ClientRpc(false);
        if(IsOwner){
            GameEvents.RebindEvents.OnFinishRebinding += OnFinishRebindingHandler_ServerRpc;
        }
    }

    [ClientRpc]
    public virtual void SetAssemblyMode_ClientRpc(bool enabled){
        if (IsOwner){
            AssemblyController.enabled = enabled;
        }
    }

    protected override void DeviceDiedHandler()
    {
        SetAssemblyMode_ClientRpc(false);
        base.DeviceDiedHandler();
    }
    [ClientRpc]
    protected override void LoadLocalDeviceClientRpc(string filename)
    {
        base.LoadLocalDeviceClientRpc(filename);
        if (IsOwner){
            AbilityRebinder = new AbilityRebinder(LocalAbilityActionMap);
        }
    }
    [ServerRpc]
    private void OnFinishRebindingHandler_ServerRpc(int entryID, string path){
        if (IsAlive.Value){
            SelfDevice.AbilityManager.SetBinding(entryID, path);
        }
    }
    private void InitAssemblyControl(){
        AssemblyController = GetComponent<AssemblyController>();
        Debug.Assert(AssemblyController != null, "AssemblyController is null");
        if (IsServer){
            AssemblyController.ServerInitialize(
                GetDraggableNetworkIDs, 
                GetConnectableNetworkIDs);
        }
        if (IsOwner){
            AssemblyController.OwnerInitialize(
                playerInput.currentActionMap.FindAction("SelectComponent"), 
                playerInput.currentActionMap.FindAction("DisconnectComponent"),
                playerInput.currentActionMap.FindAction("FlipComponent"), 
                playerInput.currentActionMap.FindAction("RotateComponent"));
        }
    }

    private ulong[] GetConnectableNetworkIDs()
    {
        var colliders = Physics2D.OverlapCircleAll(Utils.GetLocalGameObjectByNetworkID(RootNetworkObjectID.Value).transform.position, AssemblyRange);
        return colliders.Select(collider => collider.GetComponentInParent<IGameComponent>())
            .Where(component => component != null)
            .Select(component => component.NetworkObjectId)
            .ToArray();
    }
    public ulong[] GetDraggableNetworkIDs()
    {
        return GetConnectableNetworkIDs().Where(id => id != RootNetworkObjectID.Value).ToArray();
    }
}