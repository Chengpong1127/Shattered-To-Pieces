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
    private AssemblyController assemblyController;



    [ClientRpc]
    public void LocalPlayerSetup_ClientRpc()
    {
        if(Player == null){
            SetPlayer();
        }
        SetCamera();
        assemblyController = AssemblyController.CreateInstance(gameObject, GetConnectableGameObjects, inputActionsMap["Drag"], inputActionsMap["FlipComponent"]);
        assemblyController.enabled = false;
        inputActionsMap["AssemblyToggle"].started += _ => {
            assemblyController.enabled = !assemblyController.enabled;
            Debug.Log($"AssemblyController enabled: {assemblyController.enabled}");
        };
        inputActionsMap["AssemblyToggle"].Enable();
    }

    private IGameComponent[] GetConnectableGameObjects(){
        var colliders = Physics2D.OverlapCircleAll(Player.SelfDevice.RootGameComponent.BodyTransform.position, AssemblyRange);
        return colliders.Select(collider => collider.GetComponentInParent<IGameComponent>())
            .Where(component => component != null)
            .ToArray();
    }

    private async void SetCamera(){
        await WaitForLoaded();
        VirtualCamera.Follow = Player.GetTracedTransform();
    }

    private async UniTask WaitForLoaded(){
        await UniTask.WaitUntil(() => Player.IsLoaded);
    }

    private void SetPlayer(){
        var client = NetworkManager.Singleton.LocalClient;
        Player = client.PlayerObject.GetComponent<IPlayer>();
        Debug.Assert(Player != null, "Player is null");
    }
}