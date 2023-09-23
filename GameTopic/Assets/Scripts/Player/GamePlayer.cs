using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Linq;

public class GamePlayer: BasePlayer{
    [SerializeField]
    private float AssemblyRange = 5;
    private GameObject assemblyCurtain;
    private GameObject assemblyCurtainInstance;
    private PlayerInput playerInput;
    public AssemblyController AssemblyController;
    protected override void Start(){
        base.Start();
        playerInput = GetComponent<PlayerInput>();
        InitAssemblyControl();
        assemblyCurtain = ResourceManager.Instance.LoadPrefab("AssemblyCurtain");
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

    protected override void DeviceDiedHandler()
    {
        if (assemblyCurtainInstance != null){
            ToggleAssemblyClientRpc();
        }
        base.DeviceDiedHandler();
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
                playerInput.currentActionMap.FindAction("FlipComponent"), 
                playerInput.currentActionMap.FindAction("RotateComponent"));
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
}