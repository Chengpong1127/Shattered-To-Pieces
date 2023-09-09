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
    [SerializeField]



    [ClientRpc]
    public void LocalPlayerSetup_ClientRpc()
    {
        PlayerSetup();
    }
    private async void PlayerSetup(){
        SetPlayer();
        await UniTask.WaitUntil(() => Player.IsLoaded);
        SetCamera();
        Player.AbilityActionMap.Enable();
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