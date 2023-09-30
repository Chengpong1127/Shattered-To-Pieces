using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Linq;

public class GamePlayer: BasePlayer{
    [SerializeField]
    public float AssemblyRange = 5;
    private PlayerInput playerInput;
    public AssemblyController AssemblyController;
    public AbilityRebinder AbilityRebinder;
    public GameObject AssemblyUI;
    public GameObject SkillUI;
    protected override void Start(){
        base.Start();
        playerInput = GetComponent<PlayerInput>();
        InitAssemblyControl();
        TurnOffAssembly_ClientRpc();
        if(IsOwner){
            GameEvents.RebindEvents.OnFinishRebinding += OnFinishRebindingHandler_ServerRpc;
        }
    }

    [ClientRpc]
    public void ToggleAssemblyClientRpc(){
        if (IsOwner){
            AssemblyController.enabled = !AssemblyController.enabled;
            AssemblyUI.SetActive(AssemblyController.enabled);
            SkillUI.SetActive(AssemblyController.enabled);
        }
    }

    protected override void DeviceDiedHandler()
    {
        TurnOffAssembly_ClientRpc();
        base.DeviceDiedHandler();
    }
    [ClientRpc]
    private void TurnOffAssembly_ClientRpc(){
        if (IsOwner){
            AssemblyController.enabled = false;
            AssemblyUI.SetActive(false);
            SkillUI.SetActive(false);
        } else {
            SkillUI.SetActive(false);
        }
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