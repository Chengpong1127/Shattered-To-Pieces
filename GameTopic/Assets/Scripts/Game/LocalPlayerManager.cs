using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

public class LocalPlayerManager : NetworkBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;
    public IPlayer Player { get; private set; }

    [ClientRpc]
    public void SetCameraOnPlayer_ClientRpc()
    {
        if(Player == null){
            SetPlayer();
        }
        SetCamera();

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