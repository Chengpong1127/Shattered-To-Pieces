using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using System.Collections;

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
        StartCoroutine(WaitForLoadedAndSetCamera());

    }

    IEnumerator WaitForLoadedAndSetCamera(){
        yield return new WaitUntil(() => Player.IsLoaded);
        VirtualCamera.Follow = Player.GetTracedTransform();
    }

    private void SetPlayer(){
        var client = NetworkManager.Singleton.LocalClient;
        Player = client.PlayerObject.GetComponent<IPlayer>();
        Debug.Assert(Player != null, "Player is null");
    }
}