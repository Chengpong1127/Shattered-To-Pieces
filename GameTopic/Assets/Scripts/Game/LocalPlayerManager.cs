using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Linq;

public class LocalPlayerManager : NetworkBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;
    public IPlayer Player { get; private set; }
    public InputActionAsset inputActionsMap;
    public float AssemblyRange = 30f;
    public Minimap Minimap;
    private AssemblyController assemblyController;



    [ClientRpc]
    public void LocalPlayerSetup_ClientRpc()
    {
        PlayerSetup();
    }
    private async void PlayerSetup(){
        SetPlayer();
        await UniTask.WaitUntil(() => Player.IsLoaded);
        SetCamera();
        assemblyController = AssemblyController.CreateInstance(gameObject, GetConnectableGameObjects, inputActionsMap["Drag"], inputActionsMap["FlipComponent"]);
        assemblyController.enabled = false;
        inputActionsMap["AssemblyToggle"].started += _ => {
            assemblyController.enabled = !assemblyController.enabled;
            if (assemblyController.enabled) Player.DisableAbilityInput();
            else Player.EnableAbilityInput();
            Debug.Log($"AssemblyController enabled: {assemblyController.enabled}");
        };
        inputActionsMap["AssemblyToggle"].Enable();
        Player.EnableAbilityInput();
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
        Minimap.player = Player.GetTracedTransform();
    }

    private void SetPlayer(){
        var client = NetworkManager.Singleton.LocalClient;
        Player = client.PlayerObject.GetComponent<IPlayer>();
        Debug.Assert(Player != null, "Player is null");
    }
}