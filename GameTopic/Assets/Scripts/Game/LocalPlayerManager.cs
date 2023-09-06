using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Linq;
using AbilitySystem.Authoring;

public class LocalPlayerManager : NetworkBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;
    public IPlayer Player { get; private set; }
    public float AssemblyRange = 30f;
    public Minimap Minimap;
    private AssemblyController assemblyController;
    [SerializeField]
    private LocalPlayerInputManager localPlayerInputManager;



    [ClientRpc]
    public void LocalPlayerSetup_ClientRpc()
    {
        PlayerSetup();
    }
    private async void PlayerSetup(){
        SetPlayer();
        await UniTask.WaitUntil(() => Player.IsLoaded);
        SetCamera();
        localPlayerInputManager.AbilityActionMap = Player.AbilityActionMap;
        assemblyController = AssemblyController.CreateInstance(gameObject, GetConnectableGameObjects, localPlayerInputManager.DragComponentAction, localPlayerInputManager.FlipComponentAction);
        assemblyController.enabled = false;
        localPlayerInputManager.AssemblyToggleAction.started += OnAssemblyToggle;
        localPlayerInputManager.GameActionMap.Enable();
        localPlayerInputManager.AbilityActionMap.Enable();
    }
    private void OnAssemblyToggle(InputAction.CallbackContext _){
        assemblyController.enabled = !assemblyController.enabled;
        if (assemblyController.enabled) localPlayerInputManager.AbilityActionMap.Disable();
        else localPlayerInputManager.AbilityActionMap.Enable();
        Debug.Log($"AssemblyController enabled: {assemblyController.enabled}");
    }


    private IGameComponent[] GetConnectableGameObjects(){
        var colliders = Physics2D.OverlapCircleAll(Player.SelfDevice.RootGameComponent.BodyTransform.position, AssemblyRange);
        return colliders.Select(collider => collider.GetComponentInParent<IGameComponent>())
            .Where(component => component != null)
            .ToArray();
    }

    void OnDrawGizmos()
    {
        if (Player != null && Player.IsLoaded){
            if (assemblyController != null && assemblyController.enabled){
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(Player.SelfDevice.RootGameComponent.BodyTransform.position, AssemblyRange);
            }
            
        }
        
    }

    private void SetCamera(){
        VirtualCamera.Follow = Player.GetTracedTransform();
        if (Minimap != null) Minimap.player = Player.GetTracedTransform();
    }

    private void SetPlayer(){
        var client = NetworkManager.Singleton.LocalClient;
        Player = client.PlayerObject.GetComponent<IPlayer>();
        Debug.Assert(Player != null, "Player is null");
    }
}